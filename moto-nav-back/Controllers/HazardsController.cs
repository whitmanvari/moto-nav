using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Hazards;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HazardsController(IHazardReportRepository hazardRepository) : ControllerBase
{
    private readonly IHazardReportRepository _hazardRepository = hazardRepository;


    /// Belirli bir koordinatın etrafındaki aktif yol engellerini PostGIS ile filtreler.
    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<HazardResponseDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 5000)
    {
        var userLocation = new Point(longitude, latitude) { SRID = 4326 };

        var hazards = await _hazardRepository.GetHazardsNearbyAsync(userLocation, radiusMeters);

        var response = hazards.Select(h => new HazardResponseDto
        {
            Id = h.Id,
            Type = h.Type,
            Description = h.Description,
            Latitude = h.Location?.Y,
            Longitude = h.Location?.X,
            UpVotes = h.UpVotes,
            DownVotes = h.DownVotes,
            CreatedViaVoiceCommand = h.CreatedViaVoiceCommand,
            CreatedAt = h.CreatedAt
        });

        return Ok(response);
    }

    /// Yeni bir yol engeli bildirimi oluşturur (Örn: Yağ birikintisi, mıcır, çukur, radar).
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHazardDto dto)
    {
        var hazard = new HazardReport
        {
            ReporterUserId = dto.ReporterUserId,
            Type = dto.Type,
            Description = dto.Description,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            CreatedViaVoiceCommand = dto.CreatedViaVoiceCommand,
            IsActive = true
        };

        var createdHazard = await _hazardRepository.AddAsync(hazard);

        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, createdHazard.Id);
    }
}