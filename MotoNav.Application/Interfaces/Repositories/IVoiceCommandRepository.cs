using MotoNav.Domain.Entities.Operations;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IVoiceCommandRepository : IGenericRepository<VoiceCommandLog>
{
    Task<IEnumerable<VoiceCommandLog>> GetByUserIdAsync(Guid userId);
}