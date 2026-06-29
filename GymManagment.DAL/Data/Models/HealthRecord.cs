using GymManagment.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class HealthRecord : BaseEntity
{
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string? Note { get; set; }

    //public BloodType BloodType { get; set; }
    public string BloodType { get; set; } = default!;
    //LastUpdated = Updated At of Base


    #region Relationships
    public Member Member { get; set; } = default!;
    public int MemberId { get; set; }
    #endregion
}
