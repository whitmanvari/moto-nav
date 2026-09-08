using MotoNav.Domain.Entities.Navigation;

namespace MotoNav.Application.Interfaces.Repositories;

public interface ICustomRouteRepository : IGenericRepository<CustomRoute>
{
    Task<IEnumerable<CustomRoute>> GetPublicRoutesAsync();
    Task<IEnumerable<CustomRoute>> GetByUserIdAsync(Guid userId);
}