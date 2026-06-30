using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.Controllers;
[Authorize]

public class PlansController : Controller
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var plans = await _planService.GetAllPlansAsync(ct);
        return View(plans);
    }
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var plan = await _planService.GetPlanByIdAsync(id, ct);
        if (plan is null)
        {
            TempData["ErrorMessage"] = "Plan Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var result = await _planService.GetPlanToUpdateAsync(id, ct);
        if (!result.success)
        {
            TempData["ErrorMessage"] = result.error ?? "Plan Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(result.value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _planService.UpdatePlanAsync(id, model, ct);
        if (result.success)
            TempData["SuccessMessage"] = "Plan updated successfully.";
        else
            TempData["ErrorMessage"] = "Plan Failed To update";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        var result = await _planService.ToggleActivationAsync(id, ct);
        if (result.success)
            TempData["SuccessMessage"] = "Plan status changed";
        else
            TempData["ErrorMessage"] = "Failed to Toggle Plan Status";
        return RedirectToAction(nameof(Index));
    }

}
