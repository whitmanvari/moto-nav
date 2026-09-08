using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class WindHazardZoneRepository(MotoNavDbContext context) : GenericRepository<WindHazardZone>(context), IWindHazardZoneRepository
{
    public async Task<IEnumerable<WindHazardZone>> GetActiveZonesIntersectingAsync(Point location, double bufferMeters)
    {
        return await _context.WindHazardZones
            .Where(w => w.IsActive && w.Boundary != null && w.Boundary.IsWithinDistance(location, bufferMeters))
            .ToListAsync();
    }
}