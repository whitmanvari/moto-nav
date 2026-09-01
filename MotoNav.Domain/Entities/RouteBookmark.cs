using MotoNav.Domain.Common;


namespace MotoNav.Domain.Entities
{
    public class RouteBookmark: BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CustomRouteId { get; set; }
        public string? PersonalNotes { get; set; }
    }
}
