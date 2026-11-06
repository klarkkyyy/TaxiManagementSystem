using System.Text.Json.Serialization;
using static System.FormattableString;

namespace LARGA.Dispatch.Services;

public record RouteResult(double DistanceKm, double DurationMin);

/// <summary>
/// Integration 2 - OSRM routing API (public demo server). Free, no key.
/// GET /route/v1/driving/{lng1},{lat1};{lng2},{lat2}  returns  { routes: [ { distance (m), duration (s) } ] }
/// </summary>
public class RoutingService(HttpClient http)
{
    public async Task<RouteResult?> GetRouteAsync(GeoResult from, GeoResult to)
    {
        // OSRM expects longitude first, then latitude.
        string url = Invariant($"route/v1/driving/{from.Lng},{from.Lat};{to.Lng},{to.Lat}?overview=false");
        OsrmResponse? res = await http.GetFromJsonAsync<OsrmResponse>(url);
        OsrmRoute? route = res?.Routes?.FirstOrDefault();
        if (route is null) return null;

        // Meters to kilometers, seconds to minutes.
        return new RouteResult(Math.Round(route.Distance / 1000, 1), Math.Round(route.Duration / 60, 0));
    }

    private class OsrmResponse
    {
        [JsonPropertyName("routes")] public List<OsrmRoute>? Routes { get; set; }
    }

    private class OsrmRoute
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
    }
}
