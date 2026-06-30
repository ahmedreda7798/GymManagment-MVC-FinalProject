using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagment.BLL.Services.Classes;
public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAttachmentService _attachmentService;

    public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _attachmentService = attachmentService;
    }

    public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
    {
        //Upload Photo
        var uploadResult = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
        if (!uploadResult.success) return Result.Validation(uploadResult.error ?? "Failed to upload photo. Please try again.");
        var storedPhotoName = uploadResult.value!;

        // Check if email already exists
        var emailExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Email == model.Email, ct);
        if (emailExists) return Result.Validation("This email address is already registered.");

        // Check if phone already exists
        var phoneExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Phone == model.Phone, ct);
        if (phoneExists) return Result.Validation("This phone number is already registered.");

        // Else Add Member and Return Success
        var member = _mapper.Map<Member>(model);
        member.Photo = storedPhotoName;

        _unitOfWork.GetRepository<Member>().Add(member);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        if (result > 0)
        {
            return Result.OK();
        }
        else
        {
            // If saving to the database failed, delete the uploaded photo to avoid orphaned files
            _attachmentService.Delete(storedPhotoName, "MembersPhoto");
            return Result.Fail("Failed to create member. Please try again.");
        }
    }

    public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default)
    {
        var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
        var mapped = members != null
            ? _mapper.Map<IEnumerable<MemberViewModel>>(members)
            : Enumerable.Empty<MemberViewModel>();
        return Result<IEnumerable<MemberViewModel>>.OK(mapped);
    }

    public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
    {
        var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(h => h.MemberId == id, ct: ct);
        if (healthRecord == null) return Result<HealthRecordViewModel>.NotFound("Health record not found.");
        return Result<HealthRecordViewModel>.OK(_mapper.Map<HealthRecordViewModel>(healthRecord));
    }

    public async Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
    {
        var member = await _unitOfWork.MemberRepository.GetMemberWithMembershipByIdAsync(id, ct);
        if (member == null) return Result<MemberViewModel>.NotFound("Member not found.");
        return Result<MemberViewModel>.OK(_mapper.Map<MemberViewModel>(member));
    }

    public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
    {
        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
        if (member == null) return Result<MemberToUpdateViewModel>.NotFound("Member not found.");
        return Result<MemberToUpdateViewModel>.OK(_mapper.Map<MemberToUpdateViewModel>(member));
    }

    public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
    {
        var memberRepo = _unitOfWork.GetRepository<Member>();

        var member = await memberRepo.GetByIdAsync(id, ct);
        if (member == null) return Result.NotFound("Member not found.");


        var emailExists = await memberRepo.ExistsAsync(m => m.Email == model.Email && m.Id != id);
        if (emailExists) return Result.Validation("This email address is already registered.");
        var phoneExists = await memberRepo.ExistsAsync(m => m.Phone == model.Phone && m.Id != id);
        if (phoneExists) return Result.Validation("This phone number is already registered.");


        _mapper.Map(model, member);
        member.UpdatedAt = DateTime.Now;

        memberRepo.Update(member);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        return result > 0 ? Result.OK() : Result.Fail("Failed to update member details. Please try again.");
    }

    public async Task<Result> RemoveMemberAsync(int memberId, CancellationToken ct)
    {
        #region Business Rules To Delete
        //Business Rules to Enforce on Delete
        //Cannot delete members with active bookings — block the delete if the member has any active reservation / session
        //Action is permanent — no soft-delete, no undo(as the warning banner says) 
        #endregion

        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
        if (member == null) return Result.NotFound("Member Not Found.");

        var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().ExistsAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct); //Bad Behaviour : Session is a related data
        if (hasFutureBookings) return Result.Fail("Cannot delete members with active bookings.");

        _unitOfWork.GetRepository<Member>().Delete(member);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        if (result > 0)
        {
            // Delete the member's photo from disk after the DB record is gone
            if (!string.IsNullOrWhiteSpace(member.Photo))
                _attachmentService.Delete(member.Photo, "MembersPhoto");

            return Result.OK();
        }

        return Result.Fail("Failed to delete member. Please try again.");
    }
}
