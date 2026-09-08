using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class HazardReportRepository(MotoNavDbContext context) : GenericRepository<HazardReport>(context), IHazardReportRepository
{
    public async Task<IEnumerable<HazardReport>> GetHazardsNearbyAsync(Point location, double radiusMeters)
    {
        return await _context.HazardReports
            .Where(h => h.IsActive && h.Location != null && h.Location.IsWithinDistance(location, radiusMeters))
            .ToListAsync();
    }

    public async Task<HazardReport?> GetByIdWithVerificationsAsync(Guid id)
    {
        return await _context.HazardReports
            .Include(h => h.Verifications)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task VerifyHazardAsync(Guid hazardId, Guid userId, bool stillPresent, string? comment)
    {
        var hazard = await _context.HazardReports
            .Include(h => h.Verifications)
            .FirstOrDefaultAsync(h => h.Id == hazardId);

        if (hazard == null) return;

        var verification = new HazardVerification
        {
            HazardReportId = hazardId,
            UserId = userId,
            IsConfirmed = stillPresent
        };

        hazard.Verifications.Add(verification);

        if (stillPresent)
        {
            hazard.UpVotes++;
        }
        else
        {
            hazard.DownVotes++;
        }
        hazard.TotalVotes++;

        if (hazard.DownVotes - hazard.UpVotes >= 3)
        {
            hazard.IsActive = false;
        }

        await _context.SaveChangesAsync();
    }
}