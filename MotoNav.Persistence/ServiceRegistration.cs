using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Persistence.Context;
using MotoNav.Persistence.Repositories;

namespace MotoNav.Persistence;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MotoNavDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.UseNetTopologySuite() // PostGIS mekansal sorgular için zorunlu
            ));

        // Generic ve özel repository kayıtları
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IHazardReportRepository, HazardReportRepository>();
        services.AddScoped<IBikerSpotRepository, BikerSpotRepository>();
        services.AddScoped<ISosAlertRepository, SosAlertRepository>();
        services.AddScoped<ICustomRouteRepository, CustomRouteRepository>();
        services.AddScoped<ITripLogRepository, TripLogRepository>();
        services.AddScoped<IGroupRideRepository, GroupRideRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IParkingZoneRepository, ParkingZoneRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IVoiceCommandRepository, VoiceCommandRepository>();
        services.AddScoped<IWindHazardZoneRepository, WindHazardZoneRepository>();
        services.AddScoped<IRoadConditionRepository, RoadConditionRepository>();
        services.AddScoped<IMechanicReviewRepository, MechanicReviewRepository>();
        return services;
    }
}