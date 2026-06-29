using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagment.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        return View( await _bookingService.GetAllSessionsAsync(ct));
    }

    [HttpGet]
    public async Task<IActionResult> Create(int id,CancellationToken ct)
    {
        var members = await _bookingService.GetMemberForDropDownAsync(id, ct);
        ViewBag.Members = new SelectList(members, "Id", "Name");
        ViewBag.SessionId = id;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingViewModel model ,CancellationToken ct)
    {
        //if (!ModelState.IsValid) 
        var result = await _bookingService.CreateNewBookingAsync(model, ct);
        TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking Created Successfully" : result.error;
        return RedirectToAction(nameof(GetMembersForUpcomingSession),new {id = model.SessionId});
    }
     
    [HttpGet]
    public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
        => View(await _bookingService.GetMembersForUpComingBySessionIdAsync(id, ct));
    [HttpGet]
    public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken ct)
        => View(await _bookingService.GetMembersForOnGoingBySessionIdAsync(id, ct));
    [HttpPost]
    public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
    {
        var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
        TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
            result.success ? "Booking cancelled successfully." : result.error;
        return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
    }
    [HttpPost]
    public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
    {
        var result = await _bookingService.MarkAttendedAsync(memberId, sessionId, ct);
        TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
            result.success ? "Attendance Recorded" : result.error;
        return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });

    }

}
