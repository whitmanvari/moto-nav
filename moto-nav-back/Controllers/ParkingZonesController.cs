using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Spots;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;
using NetTopologySuite.Geometries;

namespace moto_nav.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParkingZonesController(IParkingZoneRepository parkingRepository) : ControllerBase
{
    private readonly IParkingZoneRepository _parkingRepository = parkingRepository;

    /// Verilen koordinat çevresindeki güvenli motosiklet park alanlarını PostGIS ile listeler.
    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<ParkingZoneResponseDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 5000)
    {
        var location = new Point(longitude, latitude) { SRID = 4326 };
        var zones = await _parkingRepository.GetNearbyParkingZonesAsync(location, radiusMeters);

        var response = zones.Select(z => new ParkingZoneResponseDto
        {
            Id = z.Id,
            ReporterUserId = z.ReporterUserId,
            Title = z.Title,
            Latitude = z.Location.Y,
            Longitude = z.Location.X,
            Type = z.Type,
            HasSecurityCamera = z.HasSecurityCamera,
            HasGroundAnchor = z.HasGroundAnchor,
            HasLighting = z.HasLighting,
            IsFree = z.IsFree,
            SafetyScore = z.SafetyScore,
            Notes = z.Notes,
            CreatedAt = z.CreatedAt
        });

        return Ok(response);
    }

    /// Yeni bir motosiklet park alanı noktası bildirir.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateParkingZoneDto dto)
    {
        var zone = new ParkingZone
        {
            ReporterUserId = dto.ReporterUserId,
            Title = dto.Title,
            Location = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            Type = dto.Type,
            HasSecurityCamera = dto.HasSecurityCamera,
            HasGroundAnchor = dto.HasGroundAnchor,
            HasLighting = dto.HasLighting,
            IsFree = dto.IsFree,
            Notes = dto.Notes,
            SafetyScore = 5.0
        };

        var created = await _parkingRepository.AddAsync(zone);
        return CreatedAtAction(nameof(GetNearby), new { latitude = dto.Latitude, longitude = dto.Longitude }, created.Id);
    }

    /// Park alanına topluluk güvenlik incelemesi / puanı ekler.
    [HttpPost("reviews")]
    public async Task<IActionResult> AddReview([FromBody] CreateParkingZoneReviewDto dto)
    {
        var review = new ParkingZoneReview
        {
            ParkingZoneId = dto.ParkingZoneId,
            ReviewerUserId = dto.ReviewerUserId,
            SafetyRating = dto.SafetyRating,
            SawSecurityCamera = dto.SawSecurityCamera,
            AttemptedTheftReported = dto.AttemptedTheftReported,
            Comment = dto.Comment
        };

        await _parkingRepository.AddReviewAsync(review);
        return Ok();
    }

    /// Belirli bir park alanına yapılan tüm yorum ve güvenlik değerlendirmelerini getirir.
    [HttpGet("{parkingZoneId:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid parkingZoneId)
    {
        var reviews = await _parkingRepository.GetReviewsByParkingZoneIdAsync(parkingZoneId);
        return Ok(reviews);
    }
}