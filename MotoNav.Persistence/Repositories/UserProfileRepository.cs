using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Users;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class UserProfileRepository(MotoNavDbContext context) : GenericRepository<UserProfile>(context), IUserProfileRepository
{
    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(u => u.Garage)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<UserProfile?> GetProfileWithGarageAsync(Guid profileId)
    {
        return await _context.UserProfiles
            .Include(u => u.Garage)
            .FirstOrDefaultAsync(u => u.Id == profileId);
    }
}