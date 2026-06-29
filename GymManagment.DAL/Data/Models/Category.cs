using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public class Category : BaseEntity
{
    public string Name { get; set; } = default!;

    #region Relationships
    public ICollection<Session> Sessions { get; set; } = default!;
    
    #endregion
}
