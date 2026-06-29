using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers;

[Authorize(Roles ="SuperAdmin,Admin")]
public class MemberShipsController : Controller
{
    private readonly IMemberShipService _memberShipService;

    public MemberShipsController(IMemberShipService memberShipService)
    {
        _memberShipService = memberShipService;
    }
    public async Task<IActionResult> Index(CancellationToken ct)
    => View(await _memberShipService.GetAllMemberShipsAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateDropDownAsync(ct);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberShipViewModel model, CancellationToken ct)
    {
        if(!ModelState.IsValid)
        {
            await PopulateDropDownAsync(ct);
            return View(model);
        }
        var result = await _memberShipService.CreateMemberShipAsync(model, ct);
        if(result.success)
        {
            TempData["SuccessMessage"] = "MemberShip Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        TempData["ErrorMessage"] = result.error;
        await PopulateDropDownAsync(ct);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _memberShipService.DeleteActiveMemberShipAsync(id, ct);
        TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "MemberShip Deleted Successfully" : result.error;
        return RedirectToAction(nameof(Index));
    }





    private async Task PopulateDropDownAsync(CancellationToken ct)
    {
        ViewBag.Plans = new SelectList(await _memberShipService.GetPlansForDropDownAsync(ct), "Id", "Name");
        ViewBag.Members = new SelectList(await _memberShipService.GetMembersForDropDownAsync(ct), "Id", "Name");
    }

}
