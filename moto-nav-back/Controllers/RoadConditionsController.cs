using System.Security.Claims;
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
public class RoadConditionsController(IRoadConditionRepository repository) : ControllerBase
{
    private readonly IRoadConditionRepository _repository = repository;

    // Belirli bir koordinatın çevresindeki aktif yol durumu bildirimlerini listeler
    [AllowAnonymous]
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 15000)
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

    // Yeni bir yol durumu bildirimi (mıcır, bozuk asfalt, yağ birikintisi vb.) ekler
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoadConditionReportDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reporterUserId = !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)
            ? parsedId
            : dto.ReporterUserId;

        var report = new RoadConditionReport
        {
            ReporterUserId = reporterUserId,
            Condition = dto.Condition,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            Description = dto.Description,
            ExpiresAt = DateTime.UtcNow.AddHours(dto.ExpiryHours > 0 ? dto.ExpiryHours : 6)
        };

        var created = await _repository.AddAsync(report);
        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, created.Id);
    }
}