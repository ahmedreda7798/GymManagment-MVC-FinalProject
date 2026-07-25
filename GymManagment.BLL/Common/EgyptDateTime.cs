namespace GymManagment.BLL.Common;

/// <summary>
/// Helper class to get the current time in Egypt Standard Time (UTC+2 / UTC+3 DST),
/// regardless of the server's configured timezone (e.g., on hosting platforms like MonsterASP).
/// Use EgyptDateTime.Now instead of DateTime.Now everywhere in business logic.
/// </summary>
public static class EgyptDateTime
{
    private static readonly TimeZoneInfo _egyptZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            // Windows uses "Egypt Standard Time", Linux/Docker uses "Africa/Cairo"
            OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");

    /// <summary>Returns the current date and time in Egypt timezone.</summary>
    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _egyptZone);
}
