using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class BikerSpotRepository(MotoNavDbContext context) : GenericRepository<BikerSpot>(context), IBikerSpotRepository
{
    public async Task<IEnumerable<BikerSpot>> GetNearbySpotsAsync(Point location, double radiusMeters)
    {
        return await _context.BikerSpots
            .Where(s => s.Location != null && s.Location.IsWithinDistance(location, radiusMeters))
            .ToListAsync();
    }
}