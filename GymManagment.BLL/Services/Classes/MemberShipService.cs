using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes;
public class MemberShipService : IMemberShipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemberShipService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
    {
        var memberExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Id == model.MemberId ,ct);
        if (!memberExists)
            return Result.NotFound("Member Not Found");
        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
        if (plan == null)
            return Result.NotFound("Plan Not Found");

        //Plan Must be Active
        if (!plan.IsActive)
            return Result.Validation("Plan Is Not Active");

        //Member Should has only one active membership
        var HasActiveMembership = await _unitOfWork.MemberShipRepository
            .ExistsAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now);

        if (HasActiveMembership) return Result.Validation("Member Already Has an Active MemberShip");

        var membership = new Membership()
        {
            MemberId = model.MemberId,
            PlanId = model.PlanId,
            CreatedAt = DateTime.Now,
            EndDate = (model.StartDate?? DateTime.Now).AddDays(plan.DurationDays) 
        };
         _unitOfWork.MemberShipRepository.Add(membership);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Create MemberShip");
        
    }

    public async Task<Result> DeleteActiveMemberShipAsync(int MemberId, CancellationToken ct = default)
    {
        var active = await  _unitOfWork.MemberShipRepository
            .FirstOrDefaultAsync(ms => ms.MemberId == MemberId &&  ms.EndDate > DateTime.UtcNow,trackin : true ,ct:ct);

        if (active == null)   return Result.NotFound("No Active MemberShip For This Member");
        _unitOfWork.MemberShipRepository.Delete(active);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed To Delete MemberShip");
    }

    public async Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync(CancellationToken ct = default)
    {
        var activeMemberships = await _unitOfWork.MemberShipRepository.GelAllMemberShipsWithMembersAndPlansAsync(m => m.EndDate > DateTime.UtcNow, ct: ct);
       return _mapper.Map<IEnumerable<MemberShipViewModel>>(activeMemberships);
    }

    public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
    {
        var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
        return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
    }

    public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
    {
        var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
        var activeplans = plans.Where(p => p.IsActive);
        return _mapper.Map<IEnumerable<PlanSelectListViewModel>>(activeplans);

    }
}
