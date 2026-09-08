namespace MotoNav.Application.DTOs.Spots;

public class CreateMechanicReviewDto
{
    public Guid BikerSpotId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? ServicedMotorcycleModel { get; set; }
    public decimal? CostEstimated { get; set; }
}