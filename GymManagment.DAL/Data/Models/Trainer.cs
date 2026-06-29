using GymManagment.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class Trainer : User
{
    //HireDate = Created At Of BaseEntity
    public Specialty Specialty { get; set; }

    #region Relationships
    public ICollection<Session> Sessions { get; set; } = default!;
    #endregion
}
