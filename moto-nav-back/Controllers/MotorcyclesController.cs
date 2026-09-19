using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Users;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Users;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MotorcyclesController(
    IMotorcycleRepository motorcycleRepository,
    IUserProfileRepository userProfileRepository) : ControllerBase
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly IUserProfileRepository _userProfileRepository = userProfileRepository;

    // Giriş yapan kullanıcının kendi garajındaki motorları getirir
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<MotorcycleResponseDto>>> GetMyGarage()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Geçersiz oturum bilgisi.");

        var profile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (profile == null)
            return NotFound("Profil bulunamadı. Lütfen önce profil oluşturun.");

        return await GetByUserProfileId(profile.Id);
    }

    // Belirtilen profil ID'sine ait garajdaki motosikletleri listeler
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

    // Kullanıcının garajına yeni motosiklet ekler
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid targetProfileId = dto.UserProfileId;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return BadRequest("Motosiklet eklemek için önce profil oluşturulmalıdır.");

            targetProfileId = profile.Id;
        }

        var bike = new Motorcycle
        {
            UserProfileId = targetProfileId,
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