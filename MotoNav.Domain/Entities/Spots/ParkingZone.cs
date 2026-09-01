using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;


namespace MotoNav.Domain.Entities.Spots
{
    public class ParkingZone : BaseEntity
    {
        public Guid ReporterUserId { get; set; }
        public string Title { get; set; } = string.Empty; // Örn: "Kadıköy Rıhtım İspark Motor Alanı"
        public Point Location { get; set; } = null!;
        public ParkingType Type { get; set; }

        // Güvenlik Kriterleri
        public bool HasSecurityCamera { get; set; } = false; // Kamera görüyor mu?
        public bool HasGroundAnchor { get; set; } = false;   // Yere kilit bağlama demiri/babası var mı?
        public bool HasLighting { get; set; } = true;        // Gece aydınlatması iyi mi?
        public bool IsFree { get; set; } = true;             // Ücretsiz mi?

        public double SafetyScore { get; set; } = 5.0;       // 1.0 - 5.0 arası topluluk güvenlik puanı
        public string? Notes { get; set; }                   // Örn: "Gece bekçisi var, güvenli."
    }
}
