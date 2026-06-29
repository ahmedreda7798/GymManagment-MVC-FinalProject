using GymManagment.DAL.Data.DataSeeding;
using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.PL;

public static class ProgramExtentions
{
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();


        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
            await dbContext.Database.MigrateAsync();
        }
        var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
        await GymDataSeeding.SeedPlansAsync(dbContext, seedFolderPath, logger);


        await IdentityDataSeediong.SeedIdentityDataAsync(roleManager ,userManager ,logger);
    }
}
