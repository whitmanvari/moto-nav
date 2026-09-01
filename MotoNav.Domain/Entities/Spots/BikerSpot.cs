using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;


namespace MotoNav.Domain.Entities.Spots
{
    public class BikerSpot: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public SpotType Type { get; set; }
        public string? Description { get; set; }
        public Point Location { get; set; } = null!;

        public bool HasBikerParking { get; set; } = true;  // Güvenli motor park alanı var mı?
        public bool HasHelmetLocker { get; set; } = false; // Kask/ekipman bırakma alanı var mı?
        public double Rating { get; set; } = 0.0;
    }
}
