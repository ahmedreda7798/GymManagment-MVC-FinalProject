using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Configurations;
public class HealthRecordConfigurations : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.Property(x => x.BloodType)
            .HasMaxLength(15);

        builder.Property(x => x.Note)
            .HasMaxLength(500);
        builder.Property(x => x.Height)
            .HasPrecision(10, 2);
        builder.Property(x => x.Weight)
            .HasPrecision(10, 2);
        builder.Property(x => x.CreatedAt) 
       .HasDefaultValueSql("GETDATE()");
    }
}
