using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Data.Models;

namespace GymManagment.BLL.Services.Interfaces;
public interface IMemberService
{
    Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default);
    Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default);
    Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);
    Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
    Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int id, CancellationToken ct = default);
    Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);
    Task<Result> RemoveMemberAsync(int id, CancellationToken ct);
}
