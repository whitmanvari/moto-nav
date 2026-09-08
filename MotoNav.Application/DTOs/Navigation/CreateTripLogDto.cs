namespace MotoNav.Application.DTOs.Navigation;

public class CreateTripLogDto
{
    public Guid UserProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public double AverageSpeed { get; set; }
    public double MaxSpeed { get; set; }
    public double? ElevationGainMeters { get; set; }
    public string? WeatherCondition { get; set; }
    public bool IsPublic { get; set; } = true;

    // Sürüş GPS çizgisi: [[lon, lat], [lon, lat], ...]
    public List<double[]> RecordedCoordinates { get; set; } = [];
}