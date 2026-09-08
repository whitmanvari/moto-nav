using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Users;

public class CreateMotorcycleDto
{
    public Guid UserProfileId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? EngineCC { get; set; }
    public MotorcycleCategory Category { get; set; }
    public bool IsPrimary { get; set; } = false;
    public string? PlateNumber { get; set; }
    public double? TankCapacityLiters { get; set; }
}