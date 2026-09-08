using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Navigation;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class CustomRouteRepository(MotoNavDbContext context) : GenericRepository<CustomRoute>(context), ICustomRouteRepository
{
    public async Task<IEnumerable<CustomRoute>> GetPublicRoutesAsync()
    {
        return await _context.CustomRoutes
            .Where(r => r.IsPublic)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CustomRoute>> GetByUserIdAsync(Guid userId)
    {
        return await _context.CustomRoutes
            .Where(r => r.CreatorUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}