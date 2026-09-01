using MotoNav.Domain.Common;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Navigation
{
    public class TripLog: BaseEntity
    {
        public Guid UserProfileId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public double DistanceKm { get; set; }
        public TimeSpan Duration { get; set; }
        public double AverageSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double? ElevationGainMeters { get; set; } //Dağ/viraj sürüşünde tırmanılan toplam irtifa (rakım farkı)
        public string? WeatherCondition { get; set; } //Sürüş sırasında hava durumu (isteğe bağlı)

        // Sürüşün tam harita çizgisi (PostGIS LineString)
        public LineString RecordedPath { get; set; } = null!;  // GPS koordinatlarıyla kaydedilen sürüş yolu

        public bool IsPublic { get; set; } = true; //Sürücü rotayı keşfet akışında paylaşmak istiyor mu, yoksa sadece kendi profilinde mi saklamak istiyor
        public int LikeCount { get; set; } = 0;
    }
}
