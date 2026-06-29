using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class Booking : BaseEntity
{
    public Member Member { get; set; } = default!;
    public int MemberId { get; set; }
    public Session Session { get; set; } = default!;
    public int SessionId { get; set; }

    //public DateTime BookingDate { get; set; } = Created At of Base Entity
    public bool IsAttended { get; set; }
}
