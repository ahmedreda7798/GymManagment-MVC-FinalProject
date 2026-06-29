using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface IMemberShipService
{
    Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync(CancellationToken ct = default);
    Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default);
    Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default);
    Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model,CancellationToken ct = default);
    Task<Result> DeleteActiveMemberShipAsync(int MemberId,CancellationToken ct = default);

}
