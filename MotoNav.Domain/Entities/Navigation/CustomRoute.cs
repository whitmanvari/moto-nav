using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Navigation;

public class CustomRoute : BaseEntity
{
    public Guid CreatorUserId { get; set; }
    public string Title { get; set; } = string.Empty; // Örn: "Şile - Ağva Sahil Virajları"
    public string? Description { get; set; }

    // Rota Çizgisi (PostGIS koordinat zinciri)
    public LineString Path { get; set; } = null!;

    public double TotalDistanceKm { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public RouteType RouteType { get; set; } = RouteType.Fastest;

    // Güvenlik ve Yol Kalitesi Metrikleri
    public double SafetyScore { get; set; } = 5.0;         // Çukur/kaza oranına göre güvenlik puanı
    public double TwistinessScore { get; set; } = 0.0;     // Viraj yoğunluğu skoru
    public bool HasDangerousWindZones { get; set; } = false; // Viyadük/köprü gibi rüzgar koridoru uyarısı

    public bool IsPublic { get; set; } = true;
}