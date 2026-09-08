using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;

namespace MotoNav.Domain.Entities.Operations
{
    public class MaintenanceTask : BaseEntity
    {
        public Guid MotorcycleId { get; set; }
        public MaintenanceType Type { get; set; }

        public double IntervalKm { get; set; }        // Kaç km'de bir yapılmalı? (Örn: 500)
        public double LastPerformedKm { get; set; }   // En son hangi km'de yapıldı?
        public DateTime? LastPerformedDate { get; set; }

        public bool TriggerOnWetRide { get; set; } = false; // Yağmurlu sürüş bittiğinde erken tetiklensin mi?
        public bool IsOverdue { get; set; } = false;        // Süresi/kilometresi geçti mi?

        public string? PreferredMechanicNote { get; set; } // Örn: "Kadıköy Moto Servis'te yapıldı"
    }
}
