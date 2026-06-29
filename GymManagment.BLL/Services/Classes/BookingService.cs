using AutoMapper;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using GymManagment.BLL.ViewModels.SessionViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes;
public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
        if (session == null) return Result.NotFound("Session Not Found");
        if (session.StartDate <= DateTime.Now) return Result.Validation("Cannot Cancel Booking For Session That Already Started");

        var booking = await _unitOfWork.BookingRepository
            .FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, trackin: true, ct);
        if (booking == null) return Result.NotFound("Booking Not Found");

        _unitOfWork.BookingRepository.Delete(booking);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Booking Cancellation Failed");

    }

    public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);
        if (session == null) return Result.NotFound("Session Not Found");
        //session should not be started
        if (session.StartDate <= DateTime.Now) return Result.Validation("Cannot Book a Session That Has Already Started");
        //member should have  -active- membership
        var memberHasActiveMembership = await _unitOfWork.MemberShipRepository.ExistsAsync(ms => ms.MemberId == model.MemberId && ms.EndDate > DateTime.UtcNow, ct);
        if (!memberHasActiveMembership ) return Result.Validation("Member Does Not Have an Active MemberShip");
        //member should book a session only once
        var alreadyBooked = await _unitOfWork.BookingRepository
            .ExistsAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId, ct);
        if (alreadyBooked)
            return Result.Validation("Member Alredy Booked This Session");
        //check session not full
        var bookedSlots = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
        if (bookedSlots >= session.Capacity)
            return Result.Validation("Session Is Full");
        _unitOfWork.BookingRepository.Add(new Booking
        {
            MemberId = model.MemberId,
            SessionId = session.Id,
            IsAttended = false,
            CreatedAt = DateTime.Now

        });
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Book Session");
    }

    public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
    {
        var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(s => s.EndDate >= DateTime.UtcNow,ct);
     

        if (!sessions.Any())
            return Enumerable.Empty<SessionViewModel>();

        var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

        foreach (var session in mappedSessions)
        {
            session.AvailableSlots = session.Capacity - (await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct));
        }

        return mappedSessions;
    }
    public async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDownAsync(int sessionId, CancellationToken ct = default)
    {
        // Get all current bookings for this specific session
        var bookings = await _unitOfWork.BookingRepository.GetAllAsync(x => x.SessionId == sessionId, tracking: false, ct);

        // Extract only the IDs of the members who already booked
        var bookedMembersIds = bookings.Select(x => x.MemberId).ToList();

        // Fetch all members except the ones who already booked
        var availableMembers = await _unitOfWork.GetRepository<Member>().GetAllAsync(x => !bookedMembersIds.Contains(x.Id), tracking: false, ct);

        // Map the available members to the dropdown view model
        return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
    }

    public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnGoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
    {
        var booking = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
        return booking.Select(b => new MemberForSessionViewModel
        {
            MemberId = b.MemberId,
            SessionId = b.SessionId,
            MemberName = b.Member.Name,
            BookingDate = b.CreatedAt.ToString("dd-MM-yyyy HH:mm")
        }).ToList();
    }

    public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingBySessionIdAsync(int sessionId, CancellationToken ct = default)
    {
        var booking = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
        return booking.Select(b => new MemberForSessionViewModel
        {
            MemberId = b.MemberId,
            SessionId = b.SessionId,
            MemberName = b.Member.Name,
            BookingDate = b.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
            IsAttended = b.IsAttended

        }).ToList();
    }

    public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
    {
        var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, trackin: true, ct);
        if (booking == null)
            return Result.NotFound("Booking Not Found");
        booking.IsAttended = true;
        booking.UpdatedAt = DateTime.Now;
        _unitOfWork.BookingRepository.Update(booking);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Mark As Attended");
    }
}
