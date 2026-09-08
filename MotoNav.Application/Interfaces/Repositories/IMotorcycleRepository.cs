using MotoNav.Domain.Entities.Users;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IMotorcycleRepository : IGenericRepository<Motorcycle>
{
    Task<IEnumerable<Motorcycle>> GetByUserProfileIdAsync(Guid userProfileId);
}