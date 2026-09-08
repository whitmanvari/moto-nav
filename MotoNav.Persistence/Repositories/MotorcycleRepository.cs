using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Users;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class MotorcycleRepository(MotoNavDbContext context) : GenericRepository<Motorcycle>(context), IMotorcycleRepository
{
    public async Task<IEnumerable<Motorcycle>> GetByUserProfileIdAsync(Guid userProfileId)
    {
        return await _context.Motorcycles
            .Where(m => m.UserProfileId == userProfileId)
            .ToListAsync();
    }
}