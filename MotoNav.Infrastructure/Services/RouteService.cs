using MotoNav.Application.DTOs.Routes;
using MotoNav.Application.Interfaces.Services;

namespace MotoNav.Infrastructure.Services;

public class RouteService : IRouteService
{
    public async Task<RouteCalculationResultDto> CalculateMotorcycleRouteAsync(CalculateRouteRequestDto request)
    {
        // 1. Örnek rota noktaları simülasyonu (Harita sağlayıcısı entegrasyonu öncesi koordinat üretimi)
        var points = GenerateSampleRoutePoints(
            request.StartLatitude, request.StartLongitude,
            request.EndLatitude, request.EndLongitude,
            request.PreferCurvyRoads);

        // 2. Toplam mesafe hesaplama (Haversine formülü)
        double totalDistanceKm = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            totalDistanceKm += CalculateDistanceKm(
                points[i].Latitude, points[i].Longitude,
                points[i + 1].Latitude, points[i + 1].Longitude);
        }

        // 3. Viraj Skoru (Twistiness) Hesaplama
        // Açı sapmalarını (bearing changes) toplayıp km başına düşen dönüş açısına göre oranlar
        double totalAngleChange = 0;
        for (int i = 0; i < points.Count - 2; i++)
        {
            var b1 = CalculateBearing(points[i], points[i + 1]);
            var b2 = CalculateBearing(points[i + 1], points[i + 2]);
            var diff = Math.Abs(b2 - b1);
            if (diff > 180) diff = 360 - diff;
            totalAngleChange += diff;
        }

        // Km başına ortalama dönüş derecesi üzerinden 0-100 normalize viraj skoru
        double anglePerKm = totalDistanceKm > 0 ? (totalAngleChange / totalDistanceKm) : 0;
        double twistinessScore = Math.Min(100, Math.Round(anglePerKm * 1.5, 1));

        string curvinessLevel = twistinessScore switch
        {
            >= 70 => "Mükemmel Virajlı (Sport / Viraj Tutkunları)",
            >= 40 => "Keyifli / Akıcı Virajlı",
            _ => "Düzlük / Hızlı Rota"
        };

        return await Task.FromResult(new RouteCalculationResultDto
        {
            TotalDistanceKm = Math.Round(totalDistanceKm, 2),
            EstimatedDurationMinutes = Math.Round(totalDistanceKm / 60.0 * 60.0, 0), // Ortalama 60 km/s baz alındı
            TwistinessScore = twistinessScore,
            CurvinessLevel = curvinessLevel,
            Coordinates = points
        });
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Dünya yarıçapı (km)
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double CalculateBearing(CoordinateDto p1, CoordinateDto p2)
    {
        var lat1 = p1.Latitude * Math.PI / 180.0;
        var lat2 = p2.Latitude * Math.PI / 180.0;
        var dLon = (p2.Longitude - p1.Longitude) * Math.PI / 180.0;

        var y = Math.Sin(dLon) * Math.Cos(lat2);
        var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        var radians = Math.Atan2(y, x);

        return (radians * 180.0 / Math.PI + 360.0) % 360.0;
    }

    private static List<CoordinateDto> GenerateSampleRoutePoints(double startLat, double startLon, double endLat, double endLon, bool curvy)
    {
        var list = new List<CoordinateDto> { new() { Latitude = startLat, Longitude = startLon } };
        int steps = curvy ? 8 : 4;

        for (int i = 1; i < steps; i++)
        {
            double ratio = (double)i / steps;
            double lat = startLat + (endLat - startLat) * ratio;
            double lon = startLon + (endLon - startLon) * ratio;

            if (curvy)
            {
                // Viraj simülasyonu için rotaya hafif zig-zag sapmaları ekleme
                double offset = (i % 2 == 0 ? 0.015 : -0.015);
                lat += offset;
                lon += offset * 0.7;
            }

            list.Add(new CoordinateDto { Latitude = lat, Longitude = lon });
        }

        list.Add(new CoordinateDto { Latitude = endLat, Longitude = endLon });
        return list;
    }
}