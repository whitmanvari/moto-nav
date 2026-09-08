namespace MotoNav.Application.DTOs.Users;

public class UserProfileResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int ReputationScore { get; set; }
    public double TotalDistanceKm { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContactPhone { get; set; }
    public List<MotorcycleResponseDto> Garage { get; set; } = [];
}