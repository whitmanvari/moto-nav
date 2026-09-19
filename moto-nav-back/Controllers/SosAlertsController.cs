using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Sos;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SosAlertsController(ISosAlertRepository sosRepository) : ControllerBase
{
    private readonly ISosAlertRepository _sosRepository = sosRepository;


    /// Çevredeki aktif ve çözülmemiş SOS çağrılarını PostGIS ile listeler (Varsayılan 25 km yarıçap).
    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<SosAlertResponseDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 25000)
    {
        var location = new Point(longitude, latitude) { SRID = 4326 };
        var alerts = await _sosRepository.GetActiveSosAlertsNearbyAsync(location, radiusMeters);

        var response = alerts.Select(a => new SosAlertResponseDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Type = a.Type,
            Latitude = a.Location.Y,
            Longitude = a.Location.X,
            Note = a.Note,
            IsResolved = a.IsResolved,
            CreatedAt = a.CreatedAt
        });

        return Ok(response);
    }


    /// Yeni bir acil durum (SOS) sinyali yayınlar.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSosAlertDto dto)
    {
        // Kullanıcı kimliğini JWT token üzerinden güvenle çekiyoruz
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reporterUserId = !string.IsNullOrEmpty(userIdClaim) ? Guid.Parse(userIdClaim) : dto.UserId;

        var alert = new SosAlert
        {
            UserId = reporterUserId,
            Type = dto.Type,
            Note = dto.Note,
            IsResolved = false,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 }
        };

        var created = await _sosRepository.AddAsync(alert);

        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, created.Id);
    }


    /// SOS çağrısını "Yardım ulaştı / Çözüldü" olarak işaretler.
    [HttpPatch("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var alert = await _sosRepository.GetByIdAsync(id);
        if (alert == null)
            return NotFound("SOS kaydı bulunamadı.");

        await _sosRepository.ResolveAlertAsync(id);
        return NoContent();
    }
}