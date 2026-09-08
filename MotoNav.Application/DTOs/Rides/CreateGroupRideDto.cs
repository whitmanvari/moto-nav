namespace MotoNav.Application.DTOs.Rides;

public class CreateGroupRideDto
{
    public Guid LeaderUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
}