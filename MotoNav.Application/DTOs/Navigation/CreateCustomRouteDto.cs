using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Navigation;

public class CreateCustomRouteDto
{
    public Guid CreatorUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RouteType RouteType { get; set; } = RouteType.Fastest;
    public double TotalDistanceKm { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public double SafetyScore { get; set; } = 5.0;
    public double TwistinessScore { get; set; } = 0.0;
    public bool HasDangerousWindZones { get; set; } = false;
    public bool IsPublic { get; set; } = true;

    // Rota koordinat çizgisi: [[lon, lat], [lon, lat], ...]
    public List<double[]> Coordinates { get; set; } = [];
}