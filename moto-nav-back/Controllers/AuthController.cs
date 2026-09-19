using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Common.Security;
using MotoNav.Application.DTOs.Auth;
using MotoNav.Application.Interfaces.Services;
using MotoNav.Domain.Entities.Users;
using MotoNav.Persistence.Context;

namespace moto_nav.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    MotoNavDbContext context,
    ITokenService tokenService) : ControllerBase
{
    private readonly MotoNavDbContext _context = context;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            return BadRequest("Bu e-posta adresi zaten kullanımda.");

        PasswordHasher.CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Email = dto.Email.ToLower(),
            PasswordHash = Convert.ToBase64String(hash),
            PasswordSalt = Convert.ToBase64String(salt),
            Role = "Rider"
        };

        await _context.Users.AddAsync(user);

        var profile = new UserProfile
        {
            UserId = user.Id,
            FullName = dto.FullName,
            BloodType = dto.BloodType,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            ReputationScore = 100,
            TotalDistanceKm = 0
        };

        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
        if (user == null)
            return Unauthorized("Geçersiz e-posta veya parola.");

        var hash = Convert.FromBase64String(user.PasswordHash);
        var salt = Convert.FromBase64String(user.PasswordSalt);

        if (!PasswordHasher.VerifyPasswordHash(dto.Password, hash, salt))
            return Unauthorized("Geçersiz e-posta veya parola.");

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }
}