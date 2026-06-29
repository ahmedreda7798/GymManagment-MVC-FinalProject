using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Configurations;
public class UserConfigurations<T> : IEntityTypeConfiguration<T> where T : User
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Name)
            .HasColumnType("varchar")
            .HasMaxLength(50);
        builder.Property(x => x.Email)
            .HasColumnType("varchar")
            .HasMaxLength(100);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();
        builder.ToTable(tb =>
        {
            tb.HasCheckConstraint("CK_User_Email", "Email LIKE '_%@_%._%'");
            tb.HasCheckConstraint("CK_User_Phone", "Phone LIKE '01[0125]%' AND len(Phone) = 11");
        });
        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Street)
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .HasColumnName("Street");

            a.Property(p => p.City)
                .HasColumnType("varchar")
                .HasMaxLength(30)
                .HasColumnName("City");
            a.Property(p => p.BuildingNumber)
                .HasColumnName("BuildingNumber");

        });


    }
}
