using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Navigation;

public class CustomRouteResponseDto
{
    public Guid Id { get; set; }
    public Guid CreatorUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RouteType RouteType { get; set; }
    public double TotalDistanceKm { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public double SafetyScore { get; set; }
    public double TwistinessScore { get; set; }
    public bool HasDangerousWindZones { get; set; }
    public bool IsPublic { get; set; }
    public List<double[]> Coordinates { get; set; } = [];
}