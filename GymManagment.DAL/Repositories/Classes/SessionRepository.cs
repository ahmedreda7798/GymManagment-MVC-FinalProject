using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes;
public class SessionRepository : GenericRepository<Session>, ISessionRepository
{
    private readonly GymDbContext _dbContext;

    public SessionRepository(GymDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct)
    {
        var query = _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
        return await query.ToListAsync();
    }
    public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(Expression<Func<Session, bool>> predicate, CancellationToken ct = default)
    {
        var query = _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
        return await query.Where(predicate).ToListAsync(ct);
    }

    public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
    {
        return await _dbContext.Bookings.AsNoTracking().CountAsync(b => b.SessionId == sessionId, ct);
        //N+1 Problem
    }

    public async Task<Session?> GetSessionByIdWithTrainerAndCategory(int id, CancellationToken ct = default)
    {
        return await _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
