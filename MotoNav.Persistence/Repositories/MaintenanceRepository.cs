using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Operations;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class MaintenanceRepository(MotoNavDbContext context) : IMaintenanceRepository
{
    private readonly MotoNavDbContext _context = context;

    public async Task<MaintenanceLog> AddLogAsync(MaintenanceLog log)
    {
        await _context.MaintenanceLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<MaintenanceLog>> GetLogsByMotorcycleIdAsync(Guid motorcycleId)
    {
        return await _context.MaintenanceLogs
            .Where(m => m.MotorcycleId == motorcycleId)
            .OrderByDescending(m => m.PerformedAtKm)
            .ToListAsync();
    }

    public async Task<MaintenanceTask> AddTaskAsync(MaintenanceTask task)
    {
        await _context.MaintenanceTasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<MaintenanceTask>> GetTasksByMotorcycleIdAsync(Guid motorcycleId)
    {
        return await _context.MaintenanceTasks
            .Where(t => t.MotorcycleId == motorcycleId)
            .ToListAsync();
    }

    public async Task UpdateTaskStatusAsync(Guid taskId, double currentKm)
    {
        var task = await _context.MaintenanceTasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task != null)
        {
            task.IsOverdue = (currentKm - task.LastPerformedKm) >= task.IntervalKm;
            await _context.SaveChangesAsync();
        }
    }
}