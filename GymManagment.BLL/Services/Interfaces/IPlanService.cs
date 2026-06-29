using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface IPlanService
{
    Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
    Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default);
    Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
    Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default);
    Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
}
