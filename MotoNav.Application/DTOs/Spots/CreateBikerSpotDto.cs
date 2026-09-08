using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Spots;

public class CreateBikerSpotDto
{
    public string Name { get; set; } = string.Empty;
    public SpotType Type { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool HasBikerParking { get; set; } = true;
    public bool HasHelmetLocker { get; set; } = false;
}