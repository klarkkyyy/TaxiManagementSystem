namespace LARGA.Dispatch.Models;

/// <summary>Current Philippine time (UTC+8), independent of the server's time zone.</summary>
public static class PhTime
{
    public static DateTime Now => DateTime.UtcNow.AddHours(8);
}
