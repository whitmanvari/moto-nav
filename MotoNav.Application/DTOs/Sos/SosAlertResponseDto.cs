using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Sos;

public class SosAlertResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SosType Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Note { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
}