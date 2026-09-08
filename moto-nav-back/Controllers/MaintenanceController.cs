using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Operations;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Operations;

namespace moto_nav.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController(IMaintenanceRepository maintenanceRepository) : ControllerBase
{
    private readonly IMaintenanceRepository _maintenanceRepository = maintenanceRepository;

    /// Motosiklete ait geçmiş bakım kayıtlarını listeler.
    [HttpGet("motorcycle/{motorcycleId:guid}/logs")]
    public async Task<ActionResult<IEnumerable<MaintenanceLogResponseDto>>> GetLogs(Guid motorcycleId)
    {
        var logs = await _maintenanceRepository.GetLogsByMotorcycleIdAsync(motorcycleId);

        var response = logs.Select(l => new MaintenanceLogResponseDto
        {
            Id = l.Id,
            MotorcycleId = l.MotorcycleId,
            BikerSpotId = l.BikerSpotId,
            Title = l.Title,
            PerformedAtKm = l.PerformedAtKm,
            Cost = l.Cost,
            Notes = l.Notes,
            ReceiptImageUrl = l.ReceiptImageUrl,
            CreatedAt = l.CreatedAt
        });

        return Ok(response);
    }

    /// Yapılan yeni bir bakımı kaydeder.
    [HttpPost("logs")]
    public async Task<IActionResult> CreateLog([FromBody] CreateMaintenanceLogDto dto)
    {
        var log = new MaintenanceLog
        {
            MotorcycleId = dto.MotorcycleId,
            BikerSpotId = dto.BikerSpotId,
            Title = dto.Title,
            PerformedAtKm = dto.PerformedAtKm,
            Cost = dto.Cost,
            Notes = dto.Notes,
            ReceiptImageUrl = dto.ReceiptImageUrl
        };

        var created = await _maintenanceRepository.AddLogAsync(log);
        return CreatedAtAction(nameof(GetLogs), new { motorcycleId = created.MotorcycleId }, created.Id);
    }

    /// Motosikletin periyodik bakım görevlerini (zincir yağlama, yağ değişimi vb.) listeler.
    [HttpGet("motorcycle/{motorcycleId:guid}/tasks")]
    public async Task<ActionResult<IEnumerable<MaintenanceTaskResponseDto>>> GetTasks(Guid motorcycleId)
    {
        var tasks = await _maintenanceRepository.GetTasksByMotorcycleIdAsync(motorcycleId);

        var response = tasks.Select(t => new MaintenanceTaskResponseDto
        {
            Id = t.Id,
            MotorcycleId = t.MotorcycleId,
            Type = t.Type,
            IntervalKm = t.IntervalKm,
            LastPerformedKm = t.LastPerformedKm,
            LastPerformedDate = t.LastPerformedDate,
            TriggerOnWetRide = t.TriggerOnWetRide,
            IsOverdue = t.IsOverdue,
            PreferredMechanicNote = t.PreferredMechanicNote
        });

        return Ok(response);
    }

    /// Motosiklet için yeni bir periyodik bakım kuralı tanımlar.
    [HttpPost("tasks")]
    public async Task<IActionResult> CreateTask([FromBody] CreateMaintenanceTaskDto dto)
    {
        var task = new MaintenanceTask
        {
            MotorcycleId = dto.MotorcycleId,
            Type = dto.Type,
            IntervalKm = dto.IntervalKm,
            LastPerformedKm = dto.LastPerformedKm,
            LastPerformedDate = dto.LastPerformedDate,
            TriggerOnWetRide = dto.TriggerOnWetRide,
            PreferredMechanicNote = dto.PreferredMechanicNote,
            IsOverdue = false
        };

        var created = await _maintenanceRepository.AddTaskAsync(task);
        return CreatedAtAction(nameof(GetTasks), new { motorcycleId = created.MotorcycleId }, created.Id);
    }
}