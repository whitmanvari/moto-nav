using MotoNav.Domain.Common;

namespace MotoNav.Domain.Entities.Operations;

public class MaintenanceLog : BaseEntity
{
    public Guid MotorcycleId { get; set; }
    public Guid? BikerSpotId { get; set; } // Hangi serviste yapıldı? (İsteğe bağlı)
    public string Title { get; set; } = string.Empty; // Örn: "10.000 km Ağır Bakımı"
    public double PerformedAtKm { get; set; }
    public decimal? Cost { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptImageUrl { get; set; } // Servis fişi / fatura görseli
}