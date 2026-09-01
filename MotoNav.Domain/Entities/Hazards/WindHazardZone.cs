using MotoNav.Domain.Common;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Hazards;

public class WindHazardZone : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Örn: "Osmangazi Köprüsü Geçişi", "Kuzey Marmara Viyadük 4"
    public Geometry Boundary { get; set; } = null!;  // Rüzgarın vurduğu köprü/viyadük geometrisi (LineString veya Polygon)
    public double AverageWindSpeedKmh { get; set; }
    public double DangerousGustSpeedKmh { get; set; } // Tehlikeli hamle/fırtına hızı (Örn: 65+ km/s)
    public string WarningMessage { get; set; } = "Şiddetli yan rüzgar: Hızınızı düşürün ve rüzgar yönüne dikkat edin.";
    public bool IsActive { get; set; } = true;
}