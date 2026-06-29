using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagment.PL.Controllers;
[Authorize(Roles = "SuperAdmin,Admin")] // add in memberships

public class SessionsController : Controller
{
    private readonly ISessionService _sessionService;

    #region Get
    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var sessions = await _sessionService.GetAllSessionsAsync(ct);
        return View(sessions);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var result = await _sessionService.GetSessionByIdAsync(id, ct);
        if (result.success)
        {
            return View(result.value);
        }
        else
        {
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

    }
    #endregion
    #region Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropDownListAsync();
            return View(model);
        }

        var result = await _sessionService.CreateSessionAsync(model, ct);
        if (result.success)
        {
            TempData["SuccessMessage"] = "Session Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = result.error;
        await PopulateDropDownListAsync();
        return View(model);


    }

    private async Task PopulateDropDownListAsync(CancellationToken ct = default)
    {
        ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
        ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(), "Id", "Name");
    }
    #endregion
    #region Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var result = await _sessionService.GetSessionToUpdate(id, ct);
        if (result.success)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            return View(result.value);
        }
        else
        {
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
    {

        if (!ModelState.IsValid)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            return View(model);
        }
        var result = await _sessionService.UpdateSessionAsync(id, model, ct);
        if (result.success)
        {
            TempData["SuccessMessage"] = "Session Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.error;
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            return View(model);
        }
    }
    #endregion
    #region Delete
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _sessionService.GetSessionByIdAsync(id, ct);
        if (result.success)
            return View(result.value);
        else
        {
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var result = await _sessionService.RemoveSessionAsync(id, ct);
        TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Session Deleted Successfully" : result.error;
        return RedirectToAction(nameof(Index));

    }
    #endregion
}
