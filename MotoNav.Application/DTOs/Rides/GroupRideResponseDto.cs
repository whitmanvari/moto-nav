namespace MotoNav.Application.DTOs.Rides;

public class GroupRideResponseDto
{
    public Guid Id { get; set; }
    public Guid LeaderUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public int MemberCount { get; set; }
    public List<Guid> MemberUserIds { get; set; } = [];
}