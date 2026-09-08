using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Hazards
{
    public class HazardReport: BaseEntity
    {
        public Guid ReporterUserId { get; set; }
        public HazardType Type { get; set; }
        public string? Description { get; set; } = null;
        //GPS Koordinat Noktası (Latitude, Longitude)
        public Point? Location { get; set; } = null;
        public int UpVotes { get; set; } = 0;
        public int DownVotes { get; set; } = 0;
        public int TotalVotes { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool CreatedViaVoiceCommand { get; set; } = false; // sürüş esnasında kask mikrofonu/ sesli komutla mı oluşturuldu?
        public ICollection<HazardVerification> Verifications { get; set; } = [];

    }
}
