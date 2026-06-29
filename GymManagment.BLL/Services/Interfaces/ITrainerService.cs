using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface ITrainerService
{
    Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
    Task<ServiceResult> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
    Task<TrainerViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default);
    Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default);
    Task<ServiceResult> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
    Task<ServiceResult> RemoveTrainerAsync(int id, CancellationToken ct = default);
}
