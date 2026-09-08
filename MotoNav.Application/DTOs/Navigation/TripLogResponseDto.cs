namespace MotoNav.Application.DTOs.Navigation;

public class TripLogResponseDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public double AverageSpeed { get; set; }
    public double MaxSpeed { get; set; }
    public double? ElevationGainMeters { get; set; }
    public string? WeatherCondition { get; set; }
    public bool IsPublic { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<double[]> RecordedCoordinates { get; set; } = [];
}