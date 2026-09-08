using MotoNav.Domain.Entities.Users;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IUserProfileRepository : IGenericRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
    Task<UserProfile?> GetProfileWithGarageAsync(Guid profileId);
}