using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Navigation;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Navigation;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripLogsController(ITripLogRepository tripLogRepository) : ControllerBase
{
    private readonly ITripLogRepository _tripLogRepository = tripLogRepository;

    /// Keşfet akışındaki paylaşılan sürüş kayıtlarını listeler.
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

    /// Bir kullanıcının profilindeki geçmiş sürüşleri getirir.
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

    /// Tamamlanan bir sürüşü ve GPS izini kaydeder.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTripLogDto dto)
    {
        if (dto.RecordedCoordinates == null || dto.RecordedCoordinates.Count < 2)
            return BadRequest("Sürüş kaydı en az 2 koordinat noktası içermelidir.");

        var coordinates = dto.RecordedCoordinates
            .Select(c => new Coordinate(c[0], c[1]))
            .ToArray();

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var lineString = geometryFactory.CreateLineString(coordinates);

        var trip = new TripLog
        {
            UserProfileId = dto.UserProfileId,
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