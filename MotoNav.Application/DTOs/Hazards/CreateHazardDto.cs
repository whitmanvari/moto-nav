using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Hazards;

public class CreateHazardDto
{
    public Guid ReporterUserId { get; set; }
    public HazardType Type { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool CreatedViaVoiceCommand { get; set; } = false;
}