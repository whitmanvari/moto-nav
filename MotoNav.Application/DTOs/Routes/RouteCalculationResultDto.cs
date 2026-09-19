namespace MotoNav.Application.DTOs.Routes;

public class RouteCalculationResultDto
{
    public double TotalDistanceKm { get; set; }
    public double EstimatedDurationMinutes { get; set; }
    public double TwistinessScore { get; set; } // 0 - 100 arası viraj skoru
    public string CurvinessLevel { get; set; } = string.Empty; // "Düzlük", "Hafif Virajlı", "Mükemmel Virajlı"
    public List<CoordinateDto> Coordinates { get; set; } = new();
}

public class CoordinateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}