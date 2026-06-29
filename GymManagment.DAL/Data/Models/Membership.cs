using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class Membership : BaseEntity
{
    public Member Member { get; set; } = default!;
    public int MemberId { get; set; }
    public Plan Plan { get; set; } = default!;
    public int PlanId { get; set; }

    //public DateTime StartDate { get; set; } StartDate = Created at of Base Entity
    public DateTime EndDate { get; set; }
    public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
    public bool IsActive => EndDate > DateTime.Now;
}
