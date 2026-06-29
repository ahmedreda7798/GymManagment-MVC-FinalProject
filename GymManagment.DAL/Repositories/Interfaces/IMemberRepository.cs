using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces;
public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetMemberWithMembershipByIdAsync(int id, CancellationToken ct = default);
}
