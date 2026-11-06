namespace LARGA.Dispatch.Models;

public enum TripStatus { Pending, Assigned, Completed, Cancelled }

/// <summary>One booking, from the moment the dispatcher creates it until it is completed.</summary>
public class Trip
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = PhTime.Now;

    // Passenger (entered by the dispatcher)
    public string PassengerName { get; set; } = "";
    public string PassengerEmail { get; set; } = "";

    // Filled in by the Geocoding API
    public string PickupAddress { get; set; } = "";
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public string DropoffAddress { get; set; } = "";
    public double DropoffLat { get; set; }
    public double DropoffLng { get; set; }

    // Filled in by the Routing API
    public double DistanceKm { get; set; }
    public double DurationMin { get; set; }

    // Calculated from route + weather + holiday
    public decimal Fare { get; set; }
    public string FareNote { get; set; } = "";

    public TripStatus Status { get; set; } = TripStatus.Pending;
    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }
}
