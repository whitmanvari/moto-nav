using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IHazardReportRepository : IGenericRepository<HazardReport>
{
    // Verilen koordinatın etrafındaki (örn. 500m çapındaki) aktif engelleri getirir
    Task<IReadOnlyList<HazardReport>> GetHazardsNearbyAsync(Point center, double radiusMeters);
}