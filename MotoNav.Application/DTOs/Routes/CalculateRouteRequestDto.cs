namespace MotoNav.Application.DTOs.Routes;

public class CalculateRouteRequestDto
{
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public bool PreferCurvyRoads { get; set; } = true;
}