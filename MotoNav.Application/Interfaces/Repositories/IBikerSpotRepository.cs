using MotoNav.Domain.Entities.Spots;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IBikerSpotRepository : IGenericRepository<BikerSpot>
{
    Task<IEnumerable<BikerSpot>> GetNearbySpotsAsync(Point location, double radiusMeters);
}