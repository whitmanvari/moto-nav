using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Operations;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class VoiceCommandRepository(MotoNavDbContext context) : GenericRepository<VoiceCommandLog>(context), IVoiceCommandRepository
{
    public async Task<IEnumerable<VoiceCommandLog>> GetByUserIdAsync(Guid userId)
    {
        return await _context.VoiceCommandLogs
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }
}