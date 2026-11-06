using System.Text.Json.Serialization;

namespace LARGA.Dispatch.Services;

/// <summary>
/// Integration 4 - Nager.Date public holidays API. Free, no key.
/// GET /api/v3/PublicHolidays/{year}/PH  returns  [ { date, localName, name } ]
/// </summary>
public class HolidayService(HttpClient http)
{
    /// <summary>Returns the holiday's name if the date is a Philippine public holiday, otherwise null.</summary>
    public async Task<string?> GetHolidayNameAsync(DateTime date)
    {
        try
        {
            List<HolidayItem>? holidays = await http.GetFromJsonAsync<List<HolidayItem>>(
                $"api/v3/PublicHolidays/{date.Year}/PH");
            return holidays?.FirstOrDefault(h => h.Date == date.ToString("yyyy-MM-dd"))?.Name;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Holiday lookup failed: {ex.Message}");
            return null;
        }
    }

    private class HolidayItem
    {
        [JsonPropertyName("date")] public string Date { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }
}
