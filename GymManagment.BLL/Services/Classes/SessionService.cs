using AutoMapper;
using AutoMapper.Execution;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.SessionViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Data.Models.Enums;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace GymManagment.BLL.Services.Classes;
public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;


    public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;

    }

    public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
    {
        #region Business Rules
        //Business Rules to Enforce
        //All fields are required — note the red asterisks
        //Start < End — end time must be after start time
        //Start must be in the future — can schedule a session in the past
        //Capacity must be a positive integer(1 -25)
        //Trainer specialty must match Category of Session — a Boxing session needs a Boxing trainer
        //Trainer must be free in that time slot — no double-booking 
        #endregion
        var startAfterEnd = model.EndDate <= model.StartDate;
        var startInPast = model.StartDate <= DateTime.Now;
        var capacityIsNegativeOrOutRange = model.Capacity <= 0 || model.Capacity > 25;

        //var errors = new Dictionary<string, string>();

        if (startAfterEnd) return Result.Validation("End Date must be after Start Date");
        if (startInPast) return Result.Validation("Start Date must be in the future");
        if (capacityIsNegativeOrOutRange) return Result.Validation("Capacity must be a positive integer between 1 and 25");

        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
        if (trainer == null)
        {
            return Result.NotFound("Trainer not found");
        }

        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
        if (category == null)
        {
            return Result.NotFound("Category not found");
        }

        var isValid = Enum.TryParse<Specialty>(category?.Name, true, out var categorySpeciality);
        if (!isValid || trainer?.Specialty != categorySpeciality)
        {
            return Result.Validation("Trainer Specialty must match Session Category");
        }


        var session = _mapper.Map<CreateSessionViewModel, Session>(model);

        _unitOfWork.SessionRepository.Add(session);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        return result > 0 ? Result.OK() : Result.Fail("Failed To Create Session");
    }

    public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct)
    {
        //var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct :ct);
        //Categot and Trainer are Included Data XXXXXXXX

        var sessionRepo = _unitOfWork.SessionRepository;

        var sessions = await sessionRepo.GetAllSessionsWithTrainerAndCategory(ct);
        if (sessions == null || !sessions.Any()) return null;
        var sessionViewModels = sessions.Select(s => new SessionViewModel
        {
            Id = s.Id,
            Capacity = s.Capacity,
            CategoryName = s.Category.Name,
            Description = s.Description,
            TrainerName = s.Trainer.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate

        });
        foreach (var session in sessionViewModels)
        {
            session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id, ct);

        }
        return sessionViewModels;
    }

    public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
    {
        var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
        return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
    }

    public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
    {
        var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
        return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
    }
    public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct)
    {
        var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(id, ct);
        if (session == null)
            return Result<SessionViewModel>.NotFound("Session Not Found");
        else
        {
            var model = _mapper.Map<SessionViewModel>(session);
            model.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.Id, ct);
            return Result<SessionViewModel>.OK(model);
        }
    }

    public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdate(int id, CancellationToken ct = default)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
        if (session == null)
            return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

        //Cannot edit an Ongoing or Completed session — only Upcoming sessions are mutable
        if (session.StartDate <= DateTime.Now)
            return Result<UpdateSessionViewModel>.Validation("Cannot Update Session That Has Already Started");

        //Cannot edit a Session that has bookings — if there are existing bookings, changing the session details could cause issues for those who have booked
        var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
        if (bookingCount > 0)
            return Result<UpdateSessionViewModel>.Validation("Cannot Update Session That Has Bookings");

        var model = _mapper.Map<UpdateSessionViewModel>(session);
        return Result<UpdateSessionViewModel>.OK(model);

    }

    public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
        if (session == null)
            return Result.NotFound("Session Not Found");

        if (session.StartDate <= DateTime.Now)
            return Result.Validation("Cannot Edit Session That Has Already Started");

        if (model.EndDate <= model.StartDate)
            return Result.Validation("End Date must be after Start Date");
        var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
        if (bookedCount > 0)
            return Result.Validation("Cannot Edit Session That Has Bookings");

        if (model.StartDate <= DateTime.Now)
            return Result.Validation("Start Date Must Be In The Future");

        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
        if (trainer == null)
        {
            return Result.NotFound("Trainer not found");
        }

        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);


        var isValid = Enum.TryParse<Specialty>(category?.Name, true, out var categorySpeciality);
        if (!isValid || trainer?.Specialty != categorySpeciality)
        {
            return Result.Validation("Trainer Specialty must match Session Category");
        }

        _mapper.Map(model, session);
        session.UpdatedAt = DateTime.Now;
        _unitOfWork.SessionRepository.Update(session);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Update Session");
    }

    public async Task<Result> RemoveSessionAsync(int id, CancellationToken ct = default)
    {
        // Fetch the session from the database
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
        if (session == null)
            return Result.NotFound("Session Not Found");

        // Cannot delete an Ongoing or Upcoming session
        if (session.EndDate >= DateTime.Now)
            return Result.Validation("Cannot Delete Session That Has Not Ended Yet");

        // 1. Fetch and delete all bookings related to this session first to avoid database conflicts
        var sessionBookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.SessionId == id, tracking: true, ct);
        foreach (var booking in sessionBookings)
        {
            _unitOfWork.BookingRepository.Delete(booking);
        }

        // 2. Now it's perfectly safe to delete the completed session
        _unitOfWork.SessionRepository.Delete(session);

        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Delete Session");
    }
}
