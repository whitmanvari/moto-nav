using MotoNav.Domain.Enums;

namespace MotoNav.Application.DTOs.Operations;

public class CreateMaintenanceTaskDto
{
    public Guid MotorcycleId { get; set; }
    public MaintenanceType Type { get; set; }
    public double IntervalKm { get; set; }
    public double LastPerformedKm { get; set; }
    public DateTime? LastPerformedDate { get; set; }
    public bool TriggerOnWetRide { get; set; } = false;
    public string? PreferredMechanicNote { get; set; }
}