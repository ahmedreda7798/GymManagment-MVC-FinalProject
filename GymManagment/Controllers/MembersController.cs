using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers;
[Authorize(Roles = "SuperAdmin")]

public class MembersController : Controller
{

    private readonly IMemberService _memberService;
    private readonly IAttachmentService _attachmentService;

    public MembersController(IMemberService memberService ,IAttachmentService attachmentService)
    {
        _memberService = memberService;
        _attachmentService = attachmentService;
    }

    //Get BaseUrl/Members/Index
    //Index - List All Members
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var result = await _memberService.GetAllMembersAsync(ct);
        return View(result.value);
    }

    //Get BaseUrl/Members/MemberDetails/{id}
    //MemberDetails - Show One Memmber Details
    public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
    {
        //Get Member By Id
        var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
        //Check if not found => Return Index With Message
        if (!result.success)
        {
            TempData["ErrorMessage"] = result.error ?? "Member Not Found";
            return RedirectToAction(nameof(Index));
        }
        //Found => Return View Data
        return View(result.value);
    }

    #region Get Member Photo
    [HttpGet]
    public async Task<IActionResult> Picture(int id, CancellationToken ct)
    {
        var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
        if (!result.success || string.IsNullOrEmpty(result.value?.Photo))
        {
            TempData["ErrorMessage"] = result.error ?? "Photo not found.";
            return RedirectToAction(nameof(Index));
        }
        var fileResult = _attachmentService.GetFile(result.value.Photo, "MembersPhoto");
        if(fileResult == null)
        {
            TempData["ErrorMessage"] = "Photo file not found.";
            return RedirectToAction(nameof(Index));
        }
        return File(fileResult.Value.stream, fileResult.Value.conteneType);
    }
    #endregion

    //Get BaseUrl/Members/HealthRecordDetails/{id}
    //HealthRecordDetails - Show One Memmber Details
    public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct) // id of member
    {
        //Get HealthRecord By Member Id
        var result = await _memberService.GetMemberHealthRecordAsync(id, ct);
        //Check if not found => Return Index With Message
        if (!result.success)
        {
            TempData["ErrorMessage"] = result.error ?? "Health Record Not Found";
            return RedirectToAction(nameof(Index));
        }
        //Found => Return View Data
        return View(result.value);
    }

    #region Create Member
    //Get BaseUrl/Members/Create
    //Create - Show Create Form
    [HttpGet]
    public IActionResult Create() => View();

    //Post BaseUrl/Members/Create {Member}
    //Create - Submit Create Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
    {
        // Check if the model is valid according to data annotations.
        //because we need No request if ModelState Is Inavlid 
        // If not, return the same view with the model to show validation errors.
        if (!ModelState.IsValid) return View(nameof(Create), model);

        //Check if the model is valid according to business rules. If not, push the error into ModelState and return the same view with the model to show it inline.
        var result = await _memberService.CreateMemberAsync(model, ct);

        if (result.success)
        {
            TempData["SuccessMessage"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(result.field ?? string.Empty, result.error!);
        return View(nameof(Create), model);
    }
    #endregion

    #region Edit Member
    //Get BaseUrl/Members/Edit/{id}
    //Edit - Disply Edit Form
    public async Task<IActionResult> EditMember(int id, CancellationToken ct)
    {
        var result = await _memberService.GetMemberToUpdateAsync(id, ct);
        if (!result.success)
        {
            TempData["ErrorMessage"] = result.error ?? "Member Not Found !";
            return RedirectToAction(nameof(Index));
        }
        return View(result.value);
    }

    //Post BaseUrl/Members/Edit {Member}
    //Edit - Submit Edit Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);

        if (result.success)
        {
            TempData["SuccessMessage"] = "Member Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        if (result.kind == ResultKind.NotFound)
        {
            TempData["ErrorMessage"] = result.error ?? "Member Not Found";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(result.field ?? string.Empty, result.error!);

        return View(model);
    }
    #endregion

    #region Delete Member
    //Get BaseUrl/Members/Delete/{id}
    //Delete - Show Form
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
        if (!result.success)
        {
            TempData["ErrorMessage"] = result.error ?? "Member Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    //Post BaseUrl/Members/DeleteCofirmed/{id}
    //DeleteCofirmed - Submit  Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
    {
        var result = await _memberService.RemoveMemberAsync(id, ct);
        if (result.success)
        {
            TempData["SuccessMessage"] = "Member Deleted Successfully";
        }
        else
        {
            TempData["ErrorMessage"] = result.error ?? "Failed to Delete Member. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion
}
