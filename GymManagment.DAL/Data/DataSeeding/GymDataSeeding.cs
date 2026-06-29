using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.DataSeeding;
public static class GymDataSeeding
{
    public static async Task SeedPlansAsync(GymDbContext dbContext ,string seedFolderPath ,ILogger logger,CancellationToken ct =default)
    {
        try
        {
            if(! await dbContext.Plans.AnyAsync(ct))
            {

                var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                if(plans.Any())
                {
                    dbContext.Plans.AddRange(plans);
                    logger.LogInformation($"Plans Seeded with Count {plans.Count}");
                }
                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(ct);
                else
                    logger.LogInformation("Plans Already Seeded");
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Gym Data Seeding Failed");
            throw;
        }
    }
    private static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
    {
        var filePath = Path.Combine(folderPath, fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed Data File Not Found : {filePath}");

        var data = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];

    }
}
