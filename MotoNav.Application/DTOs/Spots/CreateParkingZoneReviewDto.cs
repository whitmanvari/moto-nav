namespace MotoNav.Application.DTOs.Spots;

public class CreateParkingZoneReviewDto
{
    public Guid ParkingZoneId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int SafetyRating { get; set; }
    public bool SawSecurityCamera { get; set; } = false;
    public bool AttemptedTheftReported { get; set; } = false;
    public string? Comment { get; set; }
}