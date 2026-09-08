namespace MotoNav.Application.DTOs.Rides;

public class UpdateRideLocationDto
{
    public Guid GroupRideId { get; set; }
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double CurrentSpeedKmh { get; set; }
    public bool IsLaggingBehind { get; set; } = false;
}