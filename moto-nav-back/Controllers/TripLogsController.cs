using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Navigation;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Navigation;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TripLogsController(
    ITripLogRepository tripLogRepository,
    IUserProfileRepository userProfileRepository) : ControllerBase
{
    private readonly ITripLogRepository _tripLogRepository = tripLogRepository;
    private readonly IUserProfileRepository _userProfileRepository = userProfileRepository;

    /// Keşfet akışındaki herkese açık (public) sürüş kayıtlarını listeler.
    [AllowAnonymous]
    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<TripLogResponseDto>>> GetFeed()
    {
        var trips = await _tripLogRepository.GetPublicFeedAsync();

        var response = trips.Select(t => new TripLogResponseDto
        {
            Id = t.Id,
            UserProfileId = t.UserProfileId,
            Title = t.Title,
            Notes = t.Notes,
            DistanceKm = t.DistanceKm,
            Duration = t.Duration,
            AverageSpeed = t.AverageSpeed,
            MaxSpeed = t.MaxSpeed,
            ElevationGainMeters = t.ElevationGainMeters,
            WeatherCondition = t.WeatherCondition,
            IsPublic = t.IsPublic,
            LikeCount = t.LikeCount,
            CreatedAt = t.CreatedAt,
            RecordedCoordinates = t.RecordedPath?.Coordinates.Select(c => new double[] { c.X, c.Y }).ToList() ?? []
        });

        return Ok(response);
    }


    /// Giriş yapan kullanıcının kendi geçmiş sürüş kayıtlarını getirir.
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<TripLogResponseDto>>> GetMyTrips()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Geçersiz oturum bilgisi.");

        var profile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (profile == null)
            return NotFound("Profiliniz bulunamadı. Lütfen önce profil oluşturun.");

        return await GetByUserProfile(profile.Id);
    }

   
    /// Belirli bir profil ID'sine ait sürüş kayıtlarını listeler.
    [HttpGet("user/{userProfileId:guid}")]
    public async Task<ActionResult<IEnumerable<TripLogResponseDto>>> GetByUserProfile(Guid userProfileId)
    {
        var trips = await _tripLogRepository.GetByUserProfileIdAsync(userProfileId);

        var response = trips.Select(t => new TripLogResponseDto
        {
            Id = t.Id,
            UserProfileId = t.UserProfileId,
            Title = t.Title,
            Notes = t.Notes,
            DistanceKm = t.DistanceKm,
            Duration = t.Duration,
            AverageSpeed = t.AverageSpeed,
            MaxSpeed = t.MaxSpeed,
            ElevationGainMeters = t.ElevationGainMeters,
            WeatherCondition = t.WeatherCondition,
            IsPublic = t.IsPublic,
            LikeCount = t.LikeCount,
            CreatedAt = t.CreatedAt,
            RecordedCoordinates = t.RecordedPath?.Coordinates.Select(c => new double[] { c.X, c.Y }).ToList() ?? []
        });

        return Ok(response);
    }


    /// Tamamlanan bir sürüşü ve GPS izini (LineString) kaydeder.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTripLogDto dto)
    {
        if (dto.RecordedCoordinates == null || dto.RecordedCoordinates.Count < 2)
            return BadRequest("Sürüş kaydı en az 2 koordinat noktası içermelidir.");

        // Kullanıcının profiline ulaşıyoruz
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid targetProfileId = dto.UserProfileId;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return BadRequest("Sürüş kaydetmeden önce profil oluşturmalısınız (UserProfiles).");

            targetProfileId = profile.Id;
        }

        var coordinates = dto.RecordedCoordinates
            .Select(c => new Coordinate(c[0], c[1]))
            .ToArray();

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var lineString = geometryFactory.CreateLineString(coordinates);

        var trip = new TripLog
        {
            UserProfileId = targetProfileId,
            Title = dto.Title,
            Notes = dto.Notes,
            DistanceKm = dto.DistanceKm,
            Duration = TimeSpan.FromMinutes(dto.DurationMinutes),
            AverageSpeed = dto.AverageSpeed,
            MaxSpeed = dto.MaxSpeed,
            ElevationGainMeters = dto.ElevationGainMeters,
            WeatherCondition = dto.WeatherCondition,
            RecordedPath = lineString,
            IsPublic = dto.IsPublic
        };

        var created = await _tripLogRepository.AddAsync(trip);

        return CreatedAtAction(nameof(GetByUserProfile), new { userProfileId = created.UserProfileId }, created.Id);
    }
}