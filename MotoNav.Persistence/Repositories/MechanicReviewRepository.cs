using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Spots;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class MechanicReviewRepository(MotoNavDbContext context) : GenericRepository<MechanicReview>(context), IMechanicReviewRepository
{
    public async Task<IEnumerable<MechanicReview>> GetBySpotIdAsync(Guid bikerSpotId)
    {
        return await _context.MechanicReviews
            .Where(m => m.BikerSpotId == bikerSpotId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}