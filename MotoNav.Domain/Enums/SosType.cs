namespace MotoNav.Domain.Enums
{
    public enum SosType
    {
        CrashDetected = 1,     // Kaza / Devrilme Algılandı (Otomatik)
        MedicalEmergency = 2,  // Sağlık / Yaralanma Durumu (Manuel)
        FlatTire = 3,          // Lastik Patladı / İndi
        OutOfFuel = 4,         // Yakıt Bitti
        DeadBattery = 5,       // Akü Bitti / Takviye Lazım
        ChainSnap = 6,         // Zincir Koptu / Tahrik Kayışı Koptu
        MechanicalBreakdown = 7 // Genel Mekanik Arıza / Çekici Lazım

    }
}
