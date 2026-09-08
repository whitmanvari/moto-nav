using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Spots;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotsController(IBikerSpotRepository spotRepository) : ControllerBase
{
    private readonly IBikerSpotRepository _spotRepository = spotRepository;

    /// Belirli koordinat çevresindeki motosiklet dostu mekanları PostGIS ile getirir.
    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<BikerSpotResponseDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 10000)
    {
        var userLocation = new Point(longitude, latitude) { SRID = 4326 };

        var spots = await _spotRepository.GetNearbySpotsAsync(userLocation, radiusMeters);

        var response = spots.Select(s => new BikerSpotResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Type = s.Type,
            Description = s.Description,
            Latitude = s.Location?.Y,
            Longitude = s.Location?.X,
            HasBikerParking = s.HasBikerParking,
            HasHelmetLocker = s.HasHelmetLocker,
            Rating = s.Rating
        });

        return Ok(response);
    }

    /// Yeni bir motosiklet dostu mekan ekler.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBikerSpotDto dto)
    {
        var spot = new BikerSpot
        {
            Name = dto.Name,
            Type = dto.Type,
            Description = dto.Description,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            HasBikerParking = dto.HasBikerParking,
            HasHelmetLocker = dto.HasHelmetLocker
        };

        var createdSpot = await _spotRepository.AddAsync(spot);

        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, createdSpot.Id);
    }
}