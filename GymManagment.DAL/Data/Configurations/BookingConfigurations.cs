using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Configurations;
public class BookingConfigurations : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Ignore(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("BookingDate")
            .HasDefaultValueSql("GETDATE()");

        builder.HasKey(x => new { x.SessionId, x.MemberId });
    }
}
