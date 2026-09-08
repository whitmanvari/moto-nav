using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Navigation;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Navigation;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController(ICustomRouteRepository routeRepository) : ControllerBase
{
    private readonly ICustomRouteRepository _routeRepository = routeRepository;

    /// Keşfet akışındaki herkese açık rotaları listeler.
    [HttpGet("public")]
    public async Task<ActionResult<IEnumerable<CustomRouteResponseDto>>> GetPublicRoutes()
    {
        var routes = await _routeRepository.GetPublicRoutesAsync();

        var response = routes.Select(r => new CustomRouteResponseDto
        {
            Id = r.Id,
            CreatorUserId = r.CreatorUserId,
            Title = r.Title,
            Description = r.Description,
            RouteType = r.RouteType,
            TotalDistanceKm = r.TotalDistanceKm,
            EstimatedDuration = r.EstimatedDuration,
            SafetyScore = r.SafetyScore,
            TwistinessScore = r.TwistinessScore,
            HasDangerousWindZones = r.HasDangerousWindZones,
            IsPublic = r.IsPublic,
            Coordinates = r.Path?.Coordinates.Select(c => new double[] { c.X, c.Y }).ToList() ?? []
        });

        return Ok(response);
    }

    /// Yeni bir özel rota çizgisi kaydeder (LineString formatında).
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomRouteDto dto)
    {
        if (dto.Coordinates == null || dto.Coordinates.Count < 2)
            return BadRequest("Bir rota en az 2 koordinat noktası içermelidir.");

        var coordinates = dto.Coordinates
            .Select(c => new Coordinate(c[0], c[1]))
            .ToArray();

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var lineString = geometryFactory.CreateLineString(coordinates);

        var route = new CustomRoute
        {
            CreatorUserId = dto.CreatorUserId,
            Title = dto.Title,
            Description = dto.Description,
            Path = lineString,
            TotalDistanceKm = dto.TotalDistanceKm,
            EstimatedDuration = TimeSpan.FromMinutes(dto.EstimatedDurationMinutes),
            RouteType = dto.RouteType,
            SafetyScore = dto.SafetyScore,
            TwistinessScore = dto.TwistinessScore,
            HasDangerousWindZones = dto.HasDangerousWindZones,
            IsPublic = dto.IsPublic
        };

        var created = await _routeRepository.AddAsync(route);

        return CreatedAtAction(nameof(GetPublicRoutes), new { id = created.Id }, created.Id);
    }
}