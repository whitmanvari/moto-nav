

namespace MotoNav.Domain.Enums
{
    public enum MaintenanceType
    {
        ChainLube = 1,       // Zincir Temizleme & Yağlama (300-500 km veya yağmur sonrası)
        ChainTension = 2,    // Zincir Boşluğu / Gerginliği Kontrolü
        OilChange = 3,       // Motor Yağı Değişimi
        TirePressure = 4,    // Lastik Hava Basıncı
        TireReplacement = 5, // Lastik Ömrü / Değişimi
        BrakePads = 6        // Fren Balataları
    }
}
