namespace MotoNav.Application.DTOs.Users;

public class CreateUserProfileDto
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContactPhone { get; set; }
}