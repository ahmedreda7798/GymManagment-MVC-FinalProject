using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces;
public interface IMemberShipRepository : IGenericRepository<Membership>
{
    Task<IEnumerable<Membership>> GelAllMemberShipsWithMembersAndPlansAsync(Expression<Func<Membership,bool>>? predicate = null,CancellationToken ct = default);
}
