using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Sos;

public class CreateSosAlertDto
{
    public Guid UserId { get; set; }
    public SosType Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Note { get; set; }
}