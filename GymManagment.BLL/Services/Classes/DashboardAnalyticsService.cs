using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.DashboardViewModels;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes;
public class DashBoardAnalyticsService : IDashBoardAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DashBoardAnalyticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<DashboardViewModel>> GetDashBoardAnalyticsAsync(CancellationToken ct = default)
    {
        var now = DateTime.Now;

        var model = new DashboardViewModel()
        {
            TotalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct),
            ActiveMembers = await _unitOfWork.GetRepository<Membership>().CountAsync(x => x.EndDate > now, ct),
            TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct),
            UpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > now, ct),
            OngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= now && s.EndDate > now, ct),
            CompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate <= now, ct)
        };

        return Result<DashboardViewModel>.OK(model);
    }
}
