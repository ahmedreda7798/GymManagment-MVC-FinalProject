using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces;
public interface ISessionRepository : IGenericRepository<Session>
{
    Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default);
    Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(Expression<Func<Session, bool>> predicate, CancellationToken ct = default);
    Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    Task<Session?> GetSessionByIdWithTrainerAndCategory(int id, CancellationToken ct = default);
}
