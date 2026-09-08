using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Hazards;

public class HazardResponseDto
{
    public Guid Id { get; set; }
    public HazardType Type { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public bool CreatedViaVoiceCommand { get; set; }
    public DateTime CreatedAt { get; set; }
}