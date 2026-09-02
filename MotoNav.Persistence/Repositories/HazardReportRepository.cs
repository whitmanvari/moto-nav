using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class HazardReportRepository(MotoNavDbContext context)
    : GenericRepository<HazardReport>(context), IHazardReportRepository
{
    public async Task<IReadOnlyList<HazardReport>> GetHazardsNearbyAsync(Point center, double radiusMeters)
    {
        // PostGIS 'ST_DWithin' fonksiyonunu çalıştırır.
        // Verilen 'center' noktasından 'radiusMeters' kadar mesafe içindeki aktif engelleri çeker.
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.IsActive && h.Location != null && h.Location.IsWithinDistance(center, radiusMeters))
            .Include(h => h.Verifications)
            .ToListAsync();
    }
}