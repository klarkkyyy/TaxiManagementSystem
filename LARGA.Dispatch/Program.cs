using LARGA.Dispatch.Components;
using LARGA.Dispatch.Data;
using LARGA.Dispatch.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Untracked local settings (SMTP email credentials) - see appsettings.Local.json.example.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Local SQLite database (a single file, created automatically on first run).
builder.Services.AddDbContextFactory<AppDb>(o => o.UseSqlite("Data Source=dispatch.db"));

// External API integrations - each one is a small typed HttpClient service.
// Nominatim and the OSRM demo server reject requests without a User-Agent (403 Forbidden),
// so every client sends one.
const string UserAgent = "LARGA-Dispatch/1.0 (student project)";

builder.Services.AddHttpClient<GeocodingService>(c =>
{
    c.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
});
builder.Services.AddHttpClient<RoutingService>(c =>
{
    c.BaseAddress = new Uri("https://router.project-osrm.org/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
});
builder.Services.AddHttpClient<WeatherService>(c =>
{
    c.BaseAddress = new Uri("https://api.open-meteo.com/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
});
builder.Services.AddHttpClient<HolidayService>(c =>
{
    c.BaseAddress = new Uri("https://date.nager.at/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
});
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

AppDb.Initialize(app.Services.GetRequiredService<IDbContextFactory<AppDb>>());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
