using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Configurations;
public class SessionConfigurations : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_Session_Capacity", "Capacity BETWEEN 1 AND 25");
            tb.HasCheckConstraint("CK_Session_Dates", "StartDate < EndDate");
        });
    }
}
