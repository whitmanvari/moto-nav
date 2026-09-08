using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Users;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Users;

namespace moto_nav.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfilesController(IUserProfileRepository profileRepository) : ControllerBase
{
    private readonly IUserProfileRepository _profileRepository = profileRepository;

    /// Kullanıcı ID'sine göre motosikletçi profilini ve garajındaki motorları getirir.
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserProfileResponseDto>> GetByUserId(Guid userId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        if (profile == null)
            return NotFound("Kullanıcı profili bulunamadı.");

        var response = new UserProfileResponseDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.FullName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            ReputationScore = profile.ReputationScore,
            TotalDistanceKm = profile.TotalDistanceKm,
            BloodType = profile.BloodType,
            EmergencyContactPhone = profile.EmergencyContactPhone,
            Garage = profile.Garage.Select(b => new MotorcycleResponseDto
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
            }).ToList()
        };

        return Ok(response);
    }

    /// Yeni bir motosikletçi profili oluşturur.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserProfileDto dto)
    {
        var existing = await _profileRepository.GetByUserIdAsync(dto.UserId);
        if (existing != null)
            return Conflict("Bu kullanıcı için zaten bir profil mevcut.");

        var profile = new UserProfile
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            BloodType = dto.BloodType,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            ReputationScore = 100,
            TotalDistanceKm = 0
        };

        var created = await _profileRepository.AddAsync(profile);
        return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserId }, created.Id);
    }
}