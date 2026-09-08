using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Operations;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Operations;
using NetTopologySuite.Geometries;

namespace moto_nav.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoiceCommandsController(IVoiceCommandRepository repository) : ControllerBase
{
    private readonly IVoiceCommandRepository _repository = repository;

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var logs = await _repository.GetByUserIdAsync(userId);
        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> Log([FromBody] LogVoiceCommandDto dto)
    {
        var log = new VoiceCommandLog
        {
            UserId = dto.UserId,
            RawTranscribedText = dto.RawTranscribedText,
            DetectedIntent = dto.DetectedIntent,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            IsProcessedSuccessfully = dto.IsProcessedSuccessfully
        };
        var created = await _repository.AddAsync(log);
        return CreatedAtAction(nameof(GetByUser), new { userId = created.UserId }, created.Id);
    }
}