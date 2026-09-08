using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface ISosAlertRepository : IGenericRepository<SosAlert>
{
    Task<IEnumerable<SosAlert>> GetActiveSosAlertsNearbyAsync(Point location, double radiusMeters);
    Task ResolveAlertAsync(Guid id);
}
