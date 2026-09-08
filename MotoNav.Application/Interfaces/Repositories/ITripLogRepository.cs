using MotoNav.Domain.Entities.Navigation;

namespace MotoNav.Application.Interfaces.Repositories;

public interface ITripLogRepository : IGenericRepository<TripLog>
{
    Task<IEnumerable<TripLog>> GetByUserProfileIdAsync(Guid userProfileId);
    Task<IEnumerable<TripLog>> GetPublicFeedAsync();
}