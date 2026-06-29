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
public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    private readonly GymDbContext _dbContext;

    public BookingRepository(GymDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
    => await _dbContext.Bookings.AsNoTracking().Include(b => b.Member).Where(b => b.SessionId == sessionId).ToListAsync(ct);
}
