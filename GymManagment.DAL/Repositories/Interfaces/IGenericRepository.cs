using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces;
public interface IGenericRepository<TEntity> where TEntity  : BaseEntity , new()
{
    Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> GetAllAsync(
    Expression<Func<TEntity, bool>> predicate,
    bool tracking = false,
    CancellationToken ct = default);

    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);



    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default); //check if any entity exists that matches the given predicate
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackin = false, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    
}
