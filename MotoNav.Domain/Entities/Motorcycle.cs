using MotoNav.Domain.Common;
using MotoNav.Domain.Enums;

namespace MotoNav.Domain.Entities
{
    public class Motorcycle: BaseEntity
    {
        public Guid UserProfileId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? EngineCC { get; set; }
        public MotorcycleCategory Category { get; set; }
        public bool IsPrimary { get; set; } = false; //Asıl kullanılan motor mu, kayıtlı başka bir motor mu (varsayılan motor)
        public string? PlateNumber { get; set; } = string.Empty; //Plaka numarası (isteğe bağlı)
        public double? TankCapacityLiters { get; set; } //Yakıt deposu kapasitesi (isteğe bağlı) benzin menzil hesabı için 

    }
}
