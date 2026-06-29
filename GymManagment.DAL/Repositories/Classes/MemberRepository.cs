using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes;
public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    private readonly GymDbContext _dbContext;

    public MemberRepository(GymDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Member?> GetMemberWithMembershipByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.Members
            .AsNoTracking()
            .Include(m => m.Memberships)
                .ThenInclude(ms => ms.Plan)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }
}
