using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.DashboardViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces;
public interface IDashBoardAnalyticsService
{
    Task<Result<DashboardViewModel>> GetDashBoardAnalyticsAsync(CancellationToken ct = default);
}
