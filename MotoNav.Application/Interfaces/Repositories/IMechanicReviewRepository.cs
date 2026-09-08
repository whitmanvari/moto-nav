using MotoNav.Domain.Entities.Spots;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IMechanicReviewRepository : IGenericRepository<MechanicReview>
{
    Task<IEnumerable<MechanicReview>> GetBySpotIdAsync(Guid bikerSpotId);
}