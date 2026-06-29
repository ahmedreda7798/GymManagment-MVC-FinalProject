using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes;
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
{
    private readonly GymDbContext _dbContext;
    private readonly DbSet<TEntity> _set;
    public GenericRepository(GymDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<TEntity>();
    }
    public void Add(TEntity entity)
    {
        _set.Add(entity);
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync(
    Expression<Func<TEntity, bool>> predicate,
    bool tracking = false,
    CancellationToken ct = default)
    {
        IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
        return await query.Where(predicate).ToListAsync();
    }


    public void Delete(TEntity entity)
    {
        _set.Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
    {
        IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
        return await query.ToListAsync();
    }


    // Retrieves a single entity by its primary key.
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _set.FindAsync(id, ct);
    }

    public void Update(TEntity entity)
    {
        _set.Update(entity);
    }


    // Checks if at least one record exists in the database that satisfies the specified criteria..
    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        return _set.AsNoTracking().AnyAsync(predicate, ct);
    }


    // Returns the first entity that matches the specified condition, or null if no match is found.
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackin = false, CancellationToken ct = default)
    {
        IQueryable<TEntity> query = trackin ? _set : _set.AsNoTracking();
       return await query.FirstOrDefaultAsync(predicate, ct);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
    {
        if (predicate != null)
        {
            return _set.AsNoTracking().CountAsync(predicate, ct);
        }

        return _set.AsNoTracking().CountAsync(ct);
    }

    #region --
    //public async Task<TEntity?> GetByIdIncludingDeletedAsync(int id, CancellationToken ct = default)
    //=> await _set.IgnoreQueryFilters().FirstOrDefaultAsync( t => t.Id == id, ct);  

    //public async Task<bool> ExistAsync(int id, CancellationToken cancellationToken = default)
    //=> await _set.AnyAsync( t => t.Id == id, cancellationToken);  

    //public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    //=> await _set.Where(predicate).ToListAsync(ct); 
    #endregion
}
