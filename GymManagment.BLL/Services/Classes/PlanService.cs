using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagment.BLL.Services.Classes;
public class PlanService : IPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlanService(IUnitOfWork unitOfWork ,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
    {
        var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
        if (plans == null) return [];
        return plans.Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            DurationDays = p.DurationDays,
            Price = p.Price,
            IsActive = p.IsActive
        });
    }

    public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
    {
        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
        if (plan == null) return null;
        return new PlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            DurationDays = plan.DurationDays,
            Price = plan.Price,
            IsActive = plan.IsActive
        };
    }

    public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
    {
        #region Update Biseness rules
        //Plan Name cannot be updated — even if the form is tampered with, the service must reject any change to the name
        //Cannot update or deactivate a plan with active memberships — preserve the contract members signed up for 
        #endregion


        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
        if (plan is null )  
            return Result<UpdatePlanViewModel>.Validation("Plan Not Found");
        
        if (!plan.IsActive)      
            return Result<UpdatePlanViewModel>.Validation("Cannot update an inactive plan");
        
        if (await HasActiveMembershipsAsync(planId, ct))
            return Result<UpdatePlanViewModel>.Validation("Cannot update plan with active memberships");
        
        var updatePlanViewModel = _mapper.Map<Plan, UpdatePlanViewModel>(plan);
        return Result<UpdatePlanViewModel>.OK(updatePlanViewModel);

    }

    public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
    {
        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
        if (plan is null) return Result.Validation("Plan Not Found");

        if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
            return Result.Validation("Cannot deactivate plan with active memberships");

        plan.IsActive = !plan.IsActive;
        plan.UpdatedAt = DateTime.Now;
        _unitOfWork.GetRepository<Plan>().Update(plan);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() :Result.Fail("Cannot Deactivate The Plan ");
    }

    public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
    {
        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
        if (plan is null) return Result.Validation("Plan Not Found");
        if (await HasActiveMembershipsAsync(id, ct))
            return Result.Validation("Cannot Update Plan with Active Memberships");

        //Ignore PlanName update as per business rule
        _mapper.Map(model, plan);
        plan.UpdatedAt = DateTime.Now;

        _unitOfWork.GetRepository<Plan>().Update(plan);
        var result = await _unitOfWork.SaveChangesAsync(ct);
        return result > 0 ? Result.OK() : Result.Fail("Failed to update the plan");
    }

    #region Helper Method
    private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
    {
        return await _unitOfWork.GetRepository<Membership>().ExistsAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
    }
    #endregion
}
