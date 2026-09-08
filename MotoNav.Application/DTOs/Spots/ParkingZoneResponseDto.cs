using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Spots;

public class ParkingZoneResponseDto
{
    public Guid Id { get; set; }
    public Guid ReporterUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ParkingType Type { get; set; }
    public bool HasSecurityCamera { get; set; }
    public bool HasGroundAnchor { get; set; }
    public bool HasLighting { get; set; }
    public bool IsFree { get; set; }
    public double SafetyScore { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}