using MotoNav.Domain.Common;

namespace MotoNav.Domain.Entities.Spots;

public class ParkingZoneReview : BaseEntity
{
    public Guid ParkingZoneId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int SafetyRating { get; set; } // 1 - 5 arası güvenlik puanı
    public bool SawSecurityCamera { get; set; } = false;
    public bool AttemptedTheftReported { get; set; } = false; // Hırsızlık/kurcalama şüphesi var mı?
    public string? Comment { get; set; }
}