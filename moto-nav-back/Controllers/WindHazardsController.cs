using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Hazards;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WindHazardsController(IWindHazardZoneRepository repository) : ControllerBase
{
    private readonly IWindHazardZoneRepository _repository = repository;


    /// Verilen koordinatın çevresindeki (örneğin 2000 metre) aktif rüzgar tehlike bölgelerini sorgular.
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


    /// Yeni bir rüzgar tehlike bölgesi (köprü, viyadük, rüzgarlı boğaz hattı) ekler.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWindHazardZoneRequest request)
    {
        // NetTopologySuite: X = Longitude, Y = Latitude (SRID 4326 WGS84)
        var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var zone = new WindHazardZone
        {
            Name = request.Name,
            Boundary = point,
            AverageWindSpeedKmh = request.AverageWindSpeedKmh,
            DangerousGustSpeedKmh = request.DangerousGustSpeedKmh,
            WarningMessage = request.WarningMessage,
            IsActive = true
        };

        var created = await _repository.AddAsync(zone);
        return CreatedAtAction(nameof(CheckNearby), new { latitude = request.Latitude, longitude = request.Longitude }, created.Id);
    }

    public class CreateWindHazardZoneRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AverageWindSpeedKmh { get; set; }
        public double DangerousGustSpeedKmh { get; set; }
        public string WarningMessage { get; set; } = string.Empty;
    }
}