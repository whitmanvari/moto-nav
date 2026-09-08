using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Hazards;

public class CreateRoadConditionReportDto
{
    public Guid ReporterUserId { get; set; }
    public SurfaceCondition Condition { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }
    public int ExpiryHours { get; set; } = 3;
}