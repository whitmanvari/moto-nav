using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IWindHazardZoneRepository : IGenericRepository<WindHazardZone>
{
    Task<IEnumerable<WindHazardZone>> GetActiveZonesIntersectingAsync(Point location, double bufferMeters);
}