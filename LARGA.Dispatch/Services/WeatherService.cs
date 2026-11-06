using System.Globalization;
using System.Text.Json.Serialization;

namespace LARGA.Dispatch.Services;

/// <summary>Current weather shown on the dashboard, mapped from the Open-Meteo API response.</summary>
public record WeatherSnapshot(double TemperatureC, string Condition, double WindKph);

/// <summary>
/// Integration with the Open-Meteo REST API (https://open-meteo.com, free, no API key).
/// GET /v1/forecast?latitude=..&amp;longitude=..&amp;current=temperature_2m,weather_code,wind_speed_10m
/// Returns null on any failure so the dashboard still renders without weather.
/// </summary>
public class WeatherService
{
    private readonly HttpClient _http;
    private readonly string _lat;
    private readonly string _lon;

    public WeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        // Defaults to Manila; override with Weather:Latitude / Weather:Longitude.
        _lat = config["Weather:Latitude"] ?? "14.5995";
        _lon = config["Weather:Longitude"] ?? "120.9842";
    }

    public async Task<WeatherSnapshot?> GetCurrentAsync(CancellationToken ct = default)
    {
        try
        {
            string url = $"v1/forecast?latitude={_lat}&longitude={_lon}" +
                         "&current=temperature_2m,weather_code,wind_speed_10m&timezone=Asia%2FManila";
            OpenMeteoResponse? res = await _http.GetFromJsonAsync<OpenMeteoResponse>(url, ct);
            if (res?.Current is null) return null;

            return new WeatherSnapshot(
                res.Current.Temperature,
                DescribeCode(res.Current.WeatherCode),
                res.Current.WindSpeed);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Weather fetch failed: {ex.Message}");
            return null;
        }
    }

    // WMO weather interpretation codes -> readable text (data-mapping rule).
    private static string DescribeCode(int code) => code switch
    {
        0 => "Clear",
        1 or 2 or 3 => "Cloudy",
        45 or 48 => "Fog",
        >= 51 and <= 67 => "Rain",
        >= 80 and <= 82 => "Rain showers",
        >= 95 => "Thunderstorm",
        _ => "Unknown"
    };

    private class OpenMeteoResponse
    {
        [JsonPropertyName("current")] public CurrentData? Current { get; set; }
    }

    private class CurrentData
    {
        [JsonPropertyName("temperature_2m")] public double Temperature { get; set; }
        [JsonPropertyName("weather_code")] public int WeatherCode { get; set; }
        [JsonPropertyName("wind_speed_10m")] public double WindSpeed { get; set; }
    }
}
