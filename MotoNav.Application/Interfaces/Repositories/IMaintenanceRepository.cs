using MotoNav.Domain.Entities.Operations;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IMaintenanceRepository
{
    Task<MaintenanceLog> AddLogAsync(MaintenanceLog log);
    Task<IEnumerable<MaintenanceLog>> GetLogsByMotorcycleIdAsync(Guid motorcycleId);

    Task<MaintenanceTask> AddTaskAsync(MaintenanceTask task);
    Task<IEnumerable<MaintenanceTask>> GetTasksByMotorcycleIdAsync(Guid motorcycleId);
    Task UpdateTaskStatusAsync(Guid taskId, double currentKm);
}