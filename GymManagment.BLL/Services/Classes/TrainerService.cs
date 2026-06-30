using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GymManagment.BLL.Services.Classes;
public class TrainerService : ITrainerService
{
    private readonly IUnitOfWork _unitOfWork;

    public TrainerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ServiceResult> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
    {
        //Email & Phone must be unique across trainers and valid
        var emailExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Email == model.Email, ct);
        if (emailExists) return ServiceResult.Failure(nameof(model.Email), "This email address is already registered.");
        
        var phoneExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Phone == model.Phone, ct);
        if (phoneExists) return ServiceResult.Failure(nameof(model.Phone), "This phone number is already registered.");

        var trainer = new Trainer
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            DateOfBirth = model.DateOfBirth,
            Specialty = model.Specialty,
            Gender = model.Gender,
            Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street,
                City = model.City
            }
        };
        _unitOfWork.GetRepository<Trainer>().Add(trainer);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? ServiceResult.Success() : ServiceResult.Failure(string.Empty, "Failed to create trainer. Please try again.");

    }

    public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
    {
        var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);

        if (trainers == null || !trainers.Any()) return [];

        return trainers.Select(t => new TrainerViewModel
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Phone = t.Phone,
            DateOfBirth = t.DateOfBirth.ToString(),
            Gender = t.Gender.ToString(),
            Specialties = t.Specialty.ToString(),
            Address = $"{t.Address.BuildingNumber} {t.Address.Street}, {t.Address.City}"
        });
    }

    public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
        if (trainer == null) return null;

        return new TrainerViewModel
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Email = trainer.Email,
            Phone = trainer.Phone,
            DateOfBirth = trainer.DateOfBirth.ToString(),
            Gender = trainer.Gender.ToString(),
            Specialties = trainer.Specialty.ToString(),
            Address = $"{trainer.Address.BuildingNumber} {trainer.Address.Street}, {trainer.Address.City}"
        };
    }

    public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
        if (trainer == null) return null;

        return new TrainerToUpdateViewModel
        {
            Name = trainer.Name,
            Email = trainer.Email,
            Phone = trainer.Phone,
            Specialty = trainer.Specialty,
            BuildingNumber = trainer.Address.BuildingNumber,
            Street = trainer.Address.Street,
            City = trainer.Address.City
        };
    }

    public async Task<ServiceResult> RemoveTrainerAsync(int id, CancellationToken ct = default)
    {
        var trainerRepo = _unitOfWork.GetRepository<Trainer>();
        var trainer = await trainerRepo.GetByIdAsync(id, ct);
        if (trainer == null) return ServiceResult.Failure("NotFound", "Trainer Not Found");
        var hasCurrentOrFutureSessions = await _unitOfWork.GetRepository<Session>().ExistsAsync(s => s.TrainerId == trainer.Id && s.EndDate > DateTime.Now);
        if (hasCurrentOrFutureSessions) return ServiceResult.Failure("TrainerId", "Cannot Delete Trainer With Ongoing Or Upcoming Sessions");
        
        trainerRepo.Delete(trainer);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? ServiceResult.Success() : ServiceResult.Failure(string.Empty, "Failed To Delete Trainer");
    }

    public async Task<ServiceResult> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
    {
        var trainerRepo = _unitOfWork.GetRepository<Trainer>();
        var trainer = await trainerRepo.GetByIdAsync(id, ct);
        if (trainer == null) return ServiceResult.Failure("NotFound", "Trainer Not Found");

        if (trainer.Email != model.Email)
        {
            var emailExists = await trainerRepo.ExistsAsync(t => t.Email == model.Email, ct);
            if (emailExists) return ServiceResult.Failure("Email", "This email address is already registered.");
        }
        if (trainer.Phone != model.Phone)
        {
            var phoneExists = await trainerRepo.ExistsAsync(t => t.Phone == model.Phone, ct);
            if (phoneExists) return ServiceResult.Failure("Phone", "This phone number is already registered.");
        }
        
        trainer.Name = model.Name;
        trainer.Email = model.Email;
        trainer.Phone = model.Phone;
        trainer.Specialty = model.Specialty;
        trainer.Address.BuildingNumber = model.BuildingNumber;
        trainer.Address.Street = model.Street;
        trainer.Address.City = model.City;

        trainerRepo.Update(trainer);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        return result > 0 ? ServiceResult.Success() : ServiceResult.Failure(string.Empty, "Failed To Update Trainer");
    }
}
