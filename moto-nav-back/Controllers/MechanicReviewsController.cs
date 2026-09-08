using Microsoft.AspNetCore.Mvc;
using MotoNav.Application.DTOs.Spots;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;

namespace moto_nav_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MechanicReviewsController(IMechanicReviewRepository repository) : ControllerBase
{
    private readonly IMechanicReviewRepository _repository = repository;

    [HttpGet("spot/{spotId:guid}")]
    public async Task<IActionResult> GetBySpot(Guid spotId)
    {
        var reviews = await _repository.GetBySpotIdAsync(spotId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMechanicReviewDto dto)
    {
        var review = new MechanicReview
        {
            BikerSpotId = dto.BikerSpotId,
            ReviewerUserId = dto.ReviewerUserId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ServicedMotorcycleModel = dto.ServicedMotorcycleModel,
            CostEstimated = dto.CostEstimated
        };

        var created = await _repository.AddAsync(review);
        return CreatedAtAction(nameof(GetBySpot), new { spotId = created.BikerSpotId }, created.Id);
    }
}