namespace LARGA.Dispatch.Services;

public record FareEstimate(decimal Total, string Note);

/// <summary>
/// Combines the data from the other integrations into one price:
/// route distance and duration (OSRM), +10% on a public holiday (Nager.Date), +10% when raining (Open-Meteo).
/// Rates are typical Philippine taxi rates: flag-down 45, 13.50 per km, 2.00 per minute.
/// </summary>
public static class FareCalculator
{
    private const decimal FlagDown = 45m;
    private const decimal PerKm = 13.5m;
    private const decimal PerMinute = 2m;

    public static FareEstimate Calculate(double distanceKm, double durationMin, string? holidayName, bool isRaining)
    {
        decimal fare = FlagDown + (decimal)distanceKm * PerKm + (decimal)durationMin * PerMinute;
        List<string> notes = new();

        if (holidayName is not null)
        {
            fare *= 1.10m;
            notes.Add($"+10% holiday ({holidayName})");
        }

        if (isRaining)
        {
            fare *= 1.10m;
            notes.Add("+10% rain");
        }

        return new FareEstimate(Math.Round(fare, 0), string.Join(", ", notes));
    }
}
