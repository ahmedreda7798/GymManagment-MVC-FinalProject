using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.ViewModels.MemberViewModels;
public class MemberViewModel //For Index Acion
{
    public int Id { get; set; }
    public string? Photo { get; set; }
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Gender { get; set; } = default!;

    //For Member Details
    public string? DateOfBirth { get; set; } 
    public string? Address { get; set; } 
    public string? PlanName { get; set; } 
    public string? MemberShipStartDate { get; set; } 
    public string? MemberShipEndDate { get; set; } 
}
