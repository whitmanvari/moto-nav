using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Hazards;
using MotoNav.Application.Interfaces.Repositories;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WindHazardsController(IWindHazardZoneRepository repository) : ControllerBase
{
    private readonly IWindHazardZoneRepository _repository = repository;

    [HttpGet("check")]
    public async Task<ActionResult<IEnumerable<WindHazardZoneResponseDto>>> CheckNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double bufferMeters = 2000)
    {
        var point = new Point(longitude, latitude) { SRID = 4326 };
        var zones = await _repository.GetActiveZonesIntersectingAsync(point, bufferMeters);

        var response = zones.Select(z => new WindHazardZoneResponseDto
        {
            Id = z.Id,
            Name = z.Name,
            AverageWindSpeedKmh = z.AverageWindSpeedKmh,
            DangerousGustSpeedKmh = z.DangerousGustSpeedKmh,
            WarningMessage = z.WarningMessage,
            IsActive = z.IsActive
        });

        return Ok(response);
    }
}