using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces;
public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository< TEntity>() where TEntity : BaseEntity, new();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    public ISessionRepository SessionRepository { get; }
    public IMemberShipRepository MemberShipRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public IMemberRepository MemberRepository { get; }

}
