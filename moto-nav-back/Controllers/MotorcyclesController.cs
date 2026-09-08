using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Users;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Users;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorcyclesController(IMotorcycleRepository motorcycleRepository) : ControllerBase
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;

    /// Kullanıcı profiline ait garajdaki motosikletleri listeler.
    [HttpGet("user/{userProfileId:guid}")]
    public async Task<ActionResult<IEnumerable<MotorcycleResponseDto>>> GetByUserProfileId(Guid userProfileId)
    {
        var bikes = await _motorcycleRepository.GetByUserProfileIdAsync(userProfileId);

        var response = bikes.Select(b => new MotorcycleResponseDto
        {
            Id = b.Id,
            UserProfileId = b.UserProfileId,
            Brand = b.Brand,
            Model = b.Model,
            EngineCC = b.EngineCC,
            Category = b.Category,
            IsPrimary = b.IsPrimary,
            PlateNumber = b.PlateNumber,
            TankCapacityLiters = b.TankCapacityLiters
        });

        return Ok(response);
    }

    /// Kullanıcının garajına yeni motosiklet ekler.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        var bike = new Motorcycle
        {
            UserProfileId = dto.UserProfileId,
            Brand = dto.Brand,
            Model = dto.Model,
            EngineCC = dto.EngineCC,
            Category = dto.Category,
            IsPrimary = dto.IsPrimary,
            PlateNumber = dto.PlateNumber,
            TankCapacityLiters = dto.TankCapacityLiters
        };

        var createdBike = await _motorcycleRepository.AddAsync(bike);

        return CreatedAtAction(nameof(GetByUserProfileId), new { userProfileId = createdBike.UserProfileId }, createdBike.Id);
    }
}