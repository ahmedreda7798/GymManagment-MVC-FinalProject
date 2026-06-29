using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Configurations;
public class TrainerConfigurations : UserConfigurations<Trainer>, IEntityTypeConfiguration<Trainer>
{
    public new void Configure(EntityTypeBuilder<Trainer> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.CreatedAt)
           .HasColumnName("HireDate")
           .HasDefaultValueSql("GETDATE()");


    }

}
