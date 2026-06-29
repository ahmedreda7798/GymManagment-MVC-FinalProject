using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagment.PL.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashBoardAnalyticsService _dashBoardService;

    public HomeController(ILogger<HomeController> logger, IDashBoardAnalyticsService dashBoardService)
    {
        _logger = logger;
        _dashBoardService = dashBoardService;
    }

    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var result = await _dashBoardService.GetDashBoardAnalyticsAsync(ct);
        if (result.success)
        {
            return View(result.value);
        }

        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
