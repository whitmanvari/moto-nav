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
        return services;
    }
}