using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.DataSeeding;
public static class IdentityDataSeediong
{
    public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger,CancellationToken ct = default)
     {
        //Create Roles
        //Create Users
        //Assign Roles to Users

        try
        {
            bool hasUsers = await userManager.Users.AnyAsync(ct);

            #region Roles
            var roles = new List<IdentityRole>()
        {
            new IdentityRole("SuperAdmin"),
            new IdentityRole("Admin"),
            new IdentityRole("User") 
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!)) 
                {
                    var roleResult = await roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError(
                            $"Role Creation Failed For Role : {role.Name} : " +
                            $"{string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            #endregion
            #region Users
            if (!hasUsers)
            {
                var MainAdmin = new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "superadmin@gmail.com",
                    UserName = "superAdmin",
                    PhoneNumber = "01021218824"
                };
                await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                
                var Admin = new ApplicationUser()
                {
                    FirstName = "admin",
                    LastName = "",
                    Email = "admin@gmail.com",
                    UserName = "admin",
                    PhoneNumber = "01021218844"
                };
                await userManager.CreateAsync(Admin, "P@ssw0rd");
                await userManager.AddToRoleAsync(Admin, "Admin");

                logger.LogInformation($"Identity Data Seeded");
            }
            #endregion
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Identity Seeding Failed");
            return;
        }
    }
}
