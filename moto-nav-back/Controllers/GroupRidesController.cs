using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Rides;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Rides;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GroupRidesController(IGroupRideRepository rideRepository) : ControllerBase
{
    private readonly IGroupRideRepository _rideRepository = rideRepository;

    // Yeni bir grup sürüşü başlatır ve benzersiz katılım kodu (JoinCode) üretir
    [HttpPost]
    public async Task<ActionResult<GroupRideResponseDto>> Create([FromBody] CreateGroupRideDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var leaderUserId = !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)
            ? parsedId
            : dto.LeaderUserId;

        var joinCode = "MOTO-" + Random.Shared.Next(1000, 9999);

        var ride = new GroupRide
        {
            LeaderUserId = leaderUserId,
            Title = dto.Title,
            JoinCode = joinCode,
            ScheduledStartTime = dto.ScheduledStartTime,
            IsActive = true
        };

        ride.Members.Add(new GroupRideMember
        {
            GroupRideId = ride.Id,
            UserId = leaderUserId,
            IsOnline = true
        });

        var created = await _rideRepository.AddAsync(ride);

        return Ok(new GroupRideResponseDto
        {
            Id = created.Id,
            LeaderUserId = created.LeaderUserId,
            Title = created.Title,
            JoinCode = created.JoinCode,
            IsActive = created.IsActive,
            ScheduledStartTime = created.ScheduledStartTime,
            MemberCount = created.Members.Count,
            MemberUserIds = created.Members.Select(m => m.UserId).ToList()
        });
    }

    // Katılım kodu (JoinCode) ile mevcut grup sürüşüne dahil olur
    [HttpPost("join")]
    public async Task<IActionResult> Join([FromQuery] string joinCode)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Geçersiz kullanıcı oturumu.");

        var success = await _rideRepository.JoinRideAsync(joinCode, userId);
        if (!success)
            return NotFound("Geçerli veya aktif bir grup sürüşü bulunamadı.");

        return Ok("Gruba başarıyla katılındı.");
    }

    // Sürüş esnasında sürücünün anlık konumunu ve hızını günceller
    [HttpPost("location")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateRideLocationDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)
            ? parsedId
            : dto.UserId;

        var location = new GroupRideLocation
        {
            GroupRideId = dto.GroupRideId,
            UserId = userId,
            CurrentLocation = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            CurrentSpeedKmh = dto.CurrentSpeedKmh,
            IsLaggingBehind = dto.IsLaggingBehind,
            Timestamp = DateTime.UtcNow
        };

        await _rideRepository.UpdateMemberLocationAsync(location);
        return Ok();
    }

    // Gruptaki tüm üyelerin son canlı konumlarını ve hızlarını listeler
    [HttpGet("{rideId:guid}/locations")]
    public async Task<IActionResult> GetLiveLocations(Guid rideId)
    {
        var locations = await _rideRepository.GetRideLiveLocationsAsync(rideId);

        var result = locations.Select(l => new
        {
            l.UserId,
            Latitude = l.CurrentLocation?.Y,
            Longitude = l.CurrentLocation?.X,
            l.CurrentSpeedKmh,
            l.IsLaggingBehind,
            l.Timestamp
        });

        return Ok(result);
    }
}