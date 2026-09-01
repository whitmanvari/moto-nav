
using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities
{
    public class RoadConditionReport: BaseEntity
    {
        public Guid ReporterUserId { get; set; }
        public SurfaceCondition Condition { get; set; }
        public Point Location { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime ExpiresAt { get; set; } // Bu tarz hava/yol koşulları 3-4 saat sonra otomatik düşer
    }
}
