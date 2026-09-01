using MotoNav.Domain.Common;

namespace MotoNav.Domain.Entities
{
    public class UserProfile: BaseEntity
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public int ReputationScore { get; set; } = 0; //Güvenilirlik puanı , kullanıcıların katkılarına göre artar veya azalır
        public double TotalDistanceKm { get; set; } = 0;
        public string? EmergencyContactPhone { get; set; } //Acil durumda aranacak kişi ve telefon numarası (sms ile)
        public string BloodType { get; set; } = string.Empty; //Kullanıcının kan grubu (acil durumlar için)

        public ICollection<Motorcycle> Garage { get; set; } = []; //Sürücünün sahip olduğu motosikletler
        public ICollection<TripLog> SavedTrips { get; set; } = []; 
    }
}
