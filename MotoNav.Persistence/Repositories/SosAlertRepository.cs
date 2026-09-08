using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class SosAlertRepository(MotoNavDbContext context) : GenericRepository<SosAlert>(context), ISosAlertRepository
{
    public async Task<IEnumerable<SosAlert>> GetActiveSosAlertsNearbyAsync(Point location, double radiusMeters)
    {
        return await _context.SosAlerts
            .Where(s => !s.IsResolved && s.Location != null && s.Location.IsWithinDistance(location, radiusMeters))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task ResolveAlertAsync(Guid id)
    {
        var alert = await _context.SosAlerts.FirstOrDefaultAsync(s => s.Id == id);
        if (alert != null)
        {
            alert.IsResolved = true;
            await _context.SaveChangesAsync();
        }
    }
}