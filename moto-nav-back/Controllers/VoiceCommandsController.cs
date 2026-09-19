using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Operations;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Operations;
using NetTopologySuite.Geometries;

namespace moto_nav.api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VoiceCommandsController(IVoiceCommandRepository repository) : ControllerBase
{
    private readonly IVoiceCommandRepository _repository = repository;

    // Giriş yapan kullanıcının kendi sesli komut geçmişini listeler
    [HttpGet("my")]
    public async Task<IActionResult> GetMyLogs()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Geçersiz kullanıcı oturumu.");

        return await GetByUser(userId);
    }

    // Belirtilen kullanıcı ID'sine ait sesli komut kayıtlarını getirir
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var logs = await _repository.GetByUserIdAsync(userId);
        return Ok(logs);
    }

    // Sürüş esnasında algılanan sesli komutu ve konumunu günlüğe kaydeder
    [HttpPost]
    public async Task<IActionResult> Log([FromBody] LogVoiceCommandDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)
            ? parsedId
            : dto.UserId;

        var log = new VoiceCommandLog
        {
            UserId = userId,
            RawTranscribedText = dto.RawTranscribedText,
            DetectedIntent = dto.DetectedIntent,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            IsProcessedSuccessfully = dto.IsProcessedSuccessfully
        };

        var created = await _repository.AddAsync(log);
        return CreatedAtAction(nameof(GetByUser), new { userId = created.UserId }, created.Id);
    }
}