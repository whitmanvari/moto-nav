using MotoNav.Domain.Common;


namespace MotoNav.Domain.Entities
{
    public class GroupRide: BaseEntity
    {
        public Guid LeaderUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string JoinCode { get; set; } = string.Empty; // Örn: "MOTO-7821"
        public bool IsActive { get; set; } = true;
        public DateTime ScheduledStartTime { get; set; }

        public ICollection<GroupRideMember> Members { get; set; } = [];
    }

    public class GroupRideMember : BaseEntity
    {
        public Guid GroupRideId { get; set; }
        public Guid UserId { get; set; }
        public bool IsOnline { get; set; } = true;
    }
}
