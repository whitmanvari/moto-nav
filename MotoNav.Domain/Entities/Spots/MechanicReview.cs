using MotoNav.Domain.Common;

namespace MotoNav.Domain.Entities.Spots;

public class MechanicReview : BaseEntity
{
    public Guid BikerSpotId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int Rating { get; set; } // 1 - 5 yıldız
    public string? Comment { get; set; }
    public string? ServicedMotorcycleModel { get; set; } // Örn: "MT-07 zincir değişimi yapıldı"
    public decimal? CostEstimated { get; set; }          // Ortalama ödenen ücret bilgisi
}