using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using GymManagment.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface IBookingService
{
    Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);
    Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingBySessionIdAsync(int sessionId, CancellationToken ct = default);
    Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnGoingBySessionIdAsync(int sessionId, CancellationToken ct = default);
    Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default);
    Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDownAsync(int sessionId, CancellationToken ct = default);
    Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
    Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default);
}
