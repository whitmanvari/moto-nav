using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IRoadConditionRepository : IGenericRepository<RoadConditionReport>
{
    Task<IEnumerable<RoadConditionReport>> GetActiveConditionsNearbyAsync(Point location, double radiusMeters);
}