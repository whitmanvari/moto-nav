using MotoNav.Domain.Common;

namespace MotoNav.Domain.Entities.Hazards
{
    public class HazardVerification: BaseEntity
    {
        public Guid HazardReportId { get; set; }
        public HazardReport HazardReport { get; set; } = null!;
        public Guid UserId { get; set; }
        public bool IsConfirmed { get; set; } //true: Evet engel var, false: Yol temizlenmiş

    }
}
