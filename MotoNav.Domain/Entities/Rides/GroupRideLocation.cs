using MotoNav.Domain.Common;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Rides;

public class GroupRideLocation : BaseEntity
{
    public Guid GroupRideId { get; set; }
    public Guid UserId { get; set; }
    public Point CurrentLocation { get; set; } = null!;
    public double CurrentSpeedKmh { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsLaggingBehind { get; set; } = false; // Gruptan geride kaldı uyarısı
}