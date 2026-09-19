using MotoNav.Application.DTOs.Routes;

namespace MotoNav.Application.Interfaces.Services;

public interface IRouteService
{
    Task<RouteCalculationResultDto> CalculateMotorcycleRouteAsync(CalculateRouteRequestDto request);
}