using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class Member : User
{
    public string Photo { get; set; } = default!;

    //JoinDate = Created At Of BaseEntity

    #region Relationships
    public HealthRecord HealthRecord { get; set; } = default!;
    public ICollection<Membership> Memberships { get; set; } = default!;
    public ICollection<Booking> MemberSessions { get; set; } = default!;

    #endregion
}
