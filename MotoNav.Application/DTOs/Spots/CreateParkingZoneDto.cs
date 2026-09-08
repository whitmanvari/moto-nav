using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Spots;

public class CreateParkingZoneDto
{
    public Guid ReporterUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ParkingType Type { get; set; }
    public bool HasSecurityCamera { get; set; } = false;
    public bool HasGroundAnchor { get; set; } = false;
    public bool HasLighting { get; set; } = true;
    public bool IsFree { get; set; } = true;
    public string? Notes { get; set; }
}