namespace LARGA.Dispatch.Models;

/// <summary>A driver and the taxi they drive. Drivers do not log in - the dispatcher manages them.</summary>
public class Driver
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PlateNumber { get; set; } = "";
    public string TaxiModel { get; set; } = "";
}
