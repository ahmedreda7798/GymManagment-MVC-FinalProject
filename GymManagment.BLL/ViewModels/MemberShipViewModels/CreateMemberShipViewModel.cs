using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.ViewModels.MemberShipViewModels;
public class CreateMemberShipViewModel
{
    public int PlanId { get; set; }
    public int MemberId { get; set; }
    public DateTime? StartDate { get; set; }
    //public DateTime EndDate { get; set; }
}
