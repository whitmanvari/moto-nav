using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class RoadConditionRepository(MotoNavDbContext context) : GenericRepository<RoadConditionReport>(context), IRoadConditionRepository
{
    public async Task<IEnumerable<RoadConditionReport>> GetActiveConditionsNearbyAsync(Point location, double radiusMeters)
    {
        var now = DateTime.UtcNow;
        return await _context.RoadConditionReports
            .Where(r => r.ExpiresAt > now && r.Location != null && r.Location.IsWithinDistance(location, radiusMeters))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}