using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes;
public class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<string, object> _repositories = [];
    private readonly GymDbContext _dbContext;
    public ISessionRepository SessionRepository { get; }
    public IMemberShipRepository MemberShipRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public IMemberRepository MemberRepository { get; }

    public UnitOfWork(GymDbContext dbContext,
        ISessionRepository sessionRepository,
        IMemberShipRepository memberShipRepository,
        IBookingRepository bookingRepository,
        IMemberRepository memberRepository)
    {
        _dbContext = dbContext;
        SessionRepository = sessionRepository;
        MemberShipRepository = memberShipRepository;
        BookingRepository = bookingRepository;
        MemberRepository = memberRepository;
    }


    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
    {
        //Check TEntity == ??
        var typeName = typeof(TEntity).Name;
        // If Exists => return
        if (_repositories.TryGetValue(typeName, out object? value))
            return (IGenericRepository<TEntity>)value;
        //If Not => Create -> Store -> Return
        else
        {
            var repo = new GenericRepository<TEntity>(_dbContext);
            _repositories[typeName] = repo;
            return repo;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _dbContext.SaveChangesAsync(ct);
    
}
