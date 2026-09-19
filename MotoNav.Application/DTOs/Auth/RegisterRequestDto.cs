namespace MotoNav.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string BloodType { get; set; } = "0 Rh+";
    public string? EmergencyContactPhone { get; set; }
}