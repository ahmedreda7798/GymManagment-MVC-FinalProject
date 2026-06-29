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
public class MemberShipRepository : GenericRepository<Membership> ,IMemberShipRepository
{
    private readonly GymDbContext _dbContext;

    public MemberShipRepository(GymDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Membership>> GelAllMemberShipsWithMembersAndPlansAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<Membership> query = _dbContext.Memberships.AsNoTracking().Include(m => m.Member).Include(m => m.Plan);
        if (predicate is not null)
            query = query.Where(predicate);
        return await query.ToListAsync(ct);
        
    }
}
