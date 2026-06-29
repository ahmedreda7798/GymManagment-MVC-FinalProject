using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers;
[Authorize(Roles ="SuperAdmin")]

public class TrainersController : Controller
{
    private readonly ITrainerService _trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }


    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var trainers = await _trainerService.GetAllTrainersAsync(ct);
        return View(trainers);
    }
    public async Task<IActionResult> Details(int id,CancellationToken ct)
    {
        var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
        if (trainer is null)
        {
            TempData["ErrorMessage"] = "Trainer Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(trainer);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _trainerService.CreateTrainerAsync(model, ct);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Trainer Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(error.Key, error.Value);

        return View(nameof(Create), model);

    }

    
    public async Task<IActionResult> EditTrainer(int id, CancellationToken ct)
    {
        var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);
        if (trainer == null)
        {
            TempData["ErrorMessage"] = "Trainer Not Found !";
            return RedirectToAction(nameof(Index));
        }
        return View(trainer);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTrainer(int id, TrainerToUpdateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(nameof(EditTrainer), model);
        var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Trainer Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        if (result.Errors.ContainsKey("NotFound"))
        {
            TempData["ErrorMessage"] = "Trainer Not Found";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(error.Key, error.Value);

        return View(nameof(EditTrainer) , model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
        if (trainer == null)
        {
            TempData["ErrorMessage"] = "Trainer Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
    {
        var result = await _trainerService.RemoveTrainerAsync(id, ct);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Trainer Deleted Successfully";
        }
        else
        {
            TempData["ErrorMessage"] = result.Errors.Values.FirstOrDefault() ?? "Failed to Delete Trainer. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
