using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;


namespace MotoNav.Domain.Entities.Hazards
{
    public class SosAlert: BaseEntity
    {
        public Guid UserId { get; set; }
        public SosType Type { get; set; }
        public Point Location { get; set; } = null!;
        public string? Note { get; set; }
        public bool IsResolved { get; set; } = false; // Yardım ulaştı mı?
    }
}
