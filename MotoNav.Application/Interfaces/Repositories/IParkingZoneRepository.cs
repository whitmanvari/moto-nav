using MotoNav.Domain.Entities.Spots;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IParkingZoneRepository : IGenericRepository<ParkingZone>
{
    Task<IEnumerable<ParkingZone>> GetNearbyParkingZonesAsync(Point location, double radiusMeters);
    Task AddReviewAsync(ParkingZoneReview review);
    Task<IEnumerable<ParkingZoneReview>> GetReviewsByParkingZoneIdAsync(Guid parkingZoneId);
}