using GymManagment.BLL.ViewModels.AccountViewModels;
using GymManagment.BLL.ViewModels.RegisterViewModels;
using GymManagment.Controllers;
using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }


    #region LogIn
    //GET Login -> Show Form
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // If the user has a valid cookie, redirect to Home immediately
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    //Post Login -> Submit Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null, CancellationToken ct = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        //Service call to authenticate user
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError("Invalid Login", "Invalid Email Or Password");
            return View(model);
        }
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            _logger.LogInformation($"User {user.UserName} Is Signed In");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning($"User {user.UserName} Is Locked Out");

            var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
            if (lockoutEndDate.HasValue)
            {
                ViewData["LockoutEnd"] = lockoutEndDate.Value.UtcDateTime.ToString("O");
            }

            ModelState.AddModelError("Locked Account", "This Account Is Locked, Try Again in ");
            return View(model);
        }
        else
        {
            ModelState.AddModelError("Invalid Login", "Invalid Email Or Password");
            return View(model);
        }
    } 
    #endregion
    //Post LogOut
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
    //Get AccessDenied
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #region Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        // Step 1: If the user is already logged in, prevent them from seeing the registration page
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        // Step 2: Keep the returnUrl chain alive by passing it to the View via ViewData
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        // Step 1: Re-store returnUrl in ViewData so it's not lost if the validation fails and we re-render the form
        ViewData["ReturnUrl"] = returnUrl;

        // Step 2: Validate the incoming model properties based on Data Annotations
        if (!ModelState.IsValid)
            return View(model);

        // Step 3: Map the incoming ViewModel data to the core ApplicationUser entity
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        // Step 4: Attempt to create the user account in the database using Identity UserManager
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // Step 5: Assign the default role "User" to the newly registered gym member
            await _userManager.AddToRoleAsync(user, "User");

            // Step 6: Perform automatic login immediately after registration so the user doesn't have to sign in manually
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Step 7: Check if there is a pending safe destination to redirect the user back to
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Step 8: Default Fallback - If no specific target URL exists, redirect directly to the Home dashboard
            return RedirectToAction("Index", "Home");
        }

        // Step 9: If registration failed, loop through identity errors and append them to the ModelState for the View to display
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Register Failure", error.Description);
        }

        return View(model);
    }
    #endregion

}
