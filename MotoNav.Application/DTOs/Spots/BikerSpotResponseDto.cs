using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Spots;

public class BikerSpotResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SpotType Type { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool HasBikerParking { get; set; }
    public bool HasHelmetLocker { get; set; }
    public double Rating { get; set; }
}