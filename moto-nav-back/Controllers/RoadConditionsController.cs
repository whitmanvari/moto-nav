using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Hazards;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoadConditionsController(IRoadConditionRepository repository) : ControllerBase
{
    private readonly IRoadConditionRepository _repository = repository;

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusMeters = 15000)
    {
        var point = new Point(longitude, latitude) { SRID = 4326 };
        var reports = await _repository.GetActiveConditionsNearbyAsync(point, radiusMeters);

        var response = reports.Select(r => new
        {
            r.Id,
            r.ReporterUserId,
            r.Condition,
            Latitude = r.Location.Y,
            Longitude = r.Location.X,
            r.Description,
            r.ExpiresAt
        });

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoadConditionReportDto dto)
    {
        var report = new RoadConditionReport
        {
            ReporterUserId = dto.ReporterUserId,
            Condition = dto.Condition,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            Description = dto.Description,
            ExpiresAt = DateTime.UtcNow.AddHours(dto.ExpiryHours)
        };

        var created = await _repository.AddAsync(report);
        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, created.Id);
    }
}