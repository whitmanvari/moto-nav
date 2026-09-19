using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Spots;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;

namespace moto_nav_back.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MechanicReviewsController(IMechanicReviewRepository repository) : ControllerBase
{
    private readonly IMechanicReviewRepository _repository = repository;

    // Belirli bir tamirci/servis mekanına (BikerSpot) ait tüm değerlendirmeleri listeler
    [AllowAnonymous]
    [HttpGet("spot/{spotId:guid}")]
    public async Task<IActionResult> GetBySpot(Guid spotId)
    {
        var reviews = await _repository.GetBySpotIdAsync(spotId);
        return Ok(reviews);
    }

    // Bir servis/tamirci için yeni değerlendirme ve puan ekler
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMechanicReviewDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reviewerUserId = !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId)
            ? parsedId
            : dto.ReviewerUserId;

        var review = new MechanicReview
        {
            BikerSpotId = dto.BikerSpotId,
            ReviewerUserId = reviewerUserId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ServicedMotorcycleModel = dto.ServicedMotorcycleModel,
            CostEstimated = dto.CostEstimated
        };

        var created = await _repository.AddAsync(review);
        return CreatedAtAction(nameof(GetBySpot), new { spotId = created.BikerSpotId }, created.Id);
    }
}