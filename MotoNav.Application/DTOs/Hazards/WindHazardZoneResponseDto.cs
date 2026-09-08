namespace MotoNav.Application.DTOs.Hazards;

public class WindHazardZoneResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double AverageWindSpeedKmh { get; set; }
    public double DangerousGustSpeedKmh { get; set; }
    public string WarningMessage { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}