using MotoNav.Domain.Entities.Hazards;
using NetTopologySuite.Geometries;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IHazardReportRepository : IGenericRepository<HazardReport>
{
    Task<IEnumerable<HazardReport>> GetHazardsNearbyAsync(Point location, double radiusMeters);
    Task<HazardReport?> GetByIdWithVerificationsAsync(Guid id);
    Task VerifyHazardAsync(Guid hazardId, Guid userId, bool stillPresent, string? comment);
}