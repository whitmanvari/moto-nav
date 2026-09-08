using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Navigation;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class TripLogRepository(MotoNavDbContext context) : GenericRepository<TripLog>(context), ITripLogRepository
{
    public async Task<IEnumerable<TripLog>> GetByUserProfileIdAsync(Guid userProfileId)
    {
        return await _context.TripLogs
            .Where(t => t.UserProfileId == userProfileId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripLog>> GetPublicFeedAsync()
    {
        return await _context.TripLogs
            .Where(t => t.IsPublic)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}