using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;
using MotoNav.Persistence.Context;
using NetTopologySuite.Geometries;

namespace MotoNav.Persistence.Repositories;

public class ParkingZoneRepository(MotoNavDbContext context) : GenericRepository<ParkingZone>(context), IParkingZoneRepository
{
    public async Task<IEnumerable<ParkingZone>> GetNearbyParkingZonesAsync(Point location, double radiusMeters)
    {
        return await _context.ParkingZones
            .Where(p => p.Location != null && p.Location.IsWithinDistance(location, radiusMeters))
            .OrderByDescending(p => p.SafetyScore)
            .ToListAsync();
    }

    public async Task AddReviewAsync(ParkingZoneReview review)
    {
        await _context.ParkingZoneReviews.AddAsync(review);

        var zone = await _context.ParkingZones.FirstOrDefaultAsync(p => p.Id == review.ParkingZoneId);
        if (zone != null)
        {
            var allReviews = await _context.ParkingZoneReviews
                .Where(r => r.ParkingZoneId == review.ParkingZoneId)
                .Select(r => r.SafetyRating)
                .ToListAsync();

            allReviews.Add(review.SafetyRating);
            zone.SafetyScore = Math.Round(allReviews.Average(), 1);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ParkingZoneReview>> GetReviewsByParkingZoneIdAsync(Guid parkingZoneId)
    {
        return await _context.ParkingZoneReviews
            .Where(r => r.ParkingZoneId == parkingZoneId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}