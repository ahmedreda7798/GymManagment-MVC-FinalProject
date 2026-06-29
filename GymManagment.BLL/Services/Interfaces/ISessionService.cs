using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface ISessionService
{
    Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct);
    Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
    Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
    Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);
    Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct);
    Task<Result<UpdateSessionViewModel>> GetSessionToUpdate(int id, CancellationToken ct = default);
    Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
    Task<Result> RemoveSessionAsync(int id, CancellationToken ct = default);
}
