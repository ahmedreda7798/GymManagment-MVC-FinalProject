using GymManagment.DAL.Data.Configurations;
using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagment.DAL.Data.DbContexts;

public class GymDbContext : IdentityDbContext<ApplicationUser>
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {

    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("Server=.;Database=GymManagmentdb;Trusted_Connection=true;TrustServerCertificate=true");

    //}
    public DbSet<Plan> Plans { get; set; } = default!;
    public DbSet<Trainer> Trainers { get; set; } = default!;
    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Session> Sessions { get; set; } = default!;
    public DbSet<HealthRecord> HealthRecords { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Membership> Memberships { get; set; } = default!;
    public DbSet<Booking> Bookings { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}
