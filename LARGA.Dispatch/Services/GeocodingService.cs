using System.Globalization;
using static System.FormattableString;

namespace LARGA.Dispatch.Services;

public record GeoResult(double Lat, double Lng, string DisplayName);

/// <summary>
/// Integration 1 - Nominatim (OpenStreetMap) geocoding API. Free, no key.
///   Reverse (map pin to address): GET /reverse?lat=..&amp;lon=..&amp;format=json  returns  { display_name }
///   Search (place name to pin):   GET /search?q=..&amp;format=json                returns  [ { lat, lon, display_name } ]
/// Nominatim's usage policy: send a User-Agent (set in Program.cs) and make at most 1 request per second.
/// </summary>
public class GeocodingService(HttpClient http)
{
    /// <summary>Turns the coordinates of a map pin into a readable address.</summary>
    public async Task<string> ReverseAsync(double lat, double lng)
    {
        try
        {
            ReverseItem? item = await http.GetFromJsonAsync<ReverseItem>(
                Invariant($"reverse?lat={lat}&lon={lng}&format=json&zoom=18"));
            if (!string.IsNullOrWhiteSpace(item?.display_name))
            {
                return item.display_name;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Reverse geocoding failed: {ex.Message}");
        }

        // Fallback so a booking is still possible when the lookup fails.
        return Invariant($"{lat:0.00000}, {lng:0.00000}");
    }

    /// <summary>Finds a place by name so the map can jump to it. Returns null if nothing matches.</summary>
    public async Task<GeoResult?> FindAsync(string query)
    {
        string url = Invariant($"search?q={Uri.EscapeDataString(query)}&format=json&limit=1&countrycodes=ph");
        List<SearchItem>? items = await http.GetFromJsonAsync<List<SearchItem>>(url);
        SearchItem? first = items?.FirstOrDefault();
        if (first is null) return null;

        // Nominatim returns the coordinates as strings, so they are parsed into numbers here.
        return new GeoResult(
            double.Parse(first.lat, CultureInfo.InvariantCulture),
            double.Parse(first.lon, CultureInfo.InvariantCulture),
            first.display_name);
    }

    private record ReverseItem(string? display_name);
    private record SearchItem(string lat, string lon, string display_name);
}
