namespace MotoNav.Application.DTOs.Operations;

public class CreateMaintenanceLogDto
{
    public Guid MotorcycleId { get; set; }
    public Guid? BikerSpotId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double PerformedAtKm { get; set; }
    public decimal? Cost { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptImageUrl { get; set; }
}