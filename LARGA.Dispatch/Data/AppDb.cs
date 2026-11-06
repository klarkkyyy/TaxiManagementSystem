using LARGA.Dispatch.Models;
using Microsoft.EntityFrameworkCore;

namespace LARGA.Dispatch.Data;

public class AppDb(DbContextOptions<AppDb> options) : DbContext(options)
{
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Trip> Trips => Set<Trip>();

    /// <summary>Creates the SQLite file on first run and adds a few sample drivers.</summary>
    public static void Initialize(IDbContextFactory<AppDb> factory)
    {
        using AppDb db = factory.CreateDbContext();
        db.Database.EnsureCreated();

        if (!db.Drivers.Any())
        {
            db.Drivers.AddRange(
                new Driver { Name = "Juan Dela Cruz", Phone = "0917 111 2222", PlateNumber = "NBC 1234", TaxiModel = "Toyota Vios" },
                new Driver { Name = "Pedro Santos", Phone = "0918 333 4444", PlateNumber = "ABX 5678", TaxiModel = "Mitsubishi Mirage G4" },
                new Driver { Name = "Maria Reyes", Phone = "0919 555 6666", PlateNumber = "TXY 9012", TaxiModel = "Toyota Innova" });
            db.SaveChanges();
        }
    }
}
