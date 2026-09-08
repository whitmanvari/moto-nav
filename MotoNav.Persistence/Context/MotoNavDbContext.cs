using Microsoft.EntityFrameworkCore;
using MotoNav.Domain.Common;
using MotoNav.Domain.Entities.Hazards;
using MotoNav.Domain.Entities.Navigation;
using MotoNav.Domain.Entities.Operations;
using MotoNav.Domain.Entities.Rides;
using MotoNav.Domain.Entities.Spots;
using MotoNav.Domain.Entities.Users;

namespace MotoNav.Persistence.Context;

public class MotoNavDbContext(DbContextOptions<MotoNavDbContext> options) : DbContext(options)
{

    // Users
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();

    // Operations   
    public DbSet<SosAlert> SosAlerts => Set<SosAlert>();

    // Navigation
    public DbSet<CustomRoute> CustomRoutes => Set<CustomRoute>();
    public DbSet<RouteBookmark> RouteBookmarks => Set<RouteBookmark>();
    public DbSet<TripLog> TripLogs => Set<TripLog>();

    // Spots
    public DbSet<BikerSpot> BikerSpots => Set<BikerSpot>();
    public DbSet<MechanicReview> MechanicReviews => Set<MechanicReview>();
    public DbSet<ParkingZone> ParkingZones => Set<ParkingZone>();
    public DbSet<ParkingZoneReview> ParkingZoneReviews => Set<ParkingZoneReview>();

    // Rides
    public DbSet<GroupRide> GroupRides => Set<GroupRide>();
    public DbSet<GroupRideLocation> GroupRideLocations => Set<GroupRideLocation>();

    // Operations
    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
    public DbSet<MaintenanceLog> MaintenanceLogs => Set<MaintenanceLog>();
    public DbSet<VoiceCommandLog> VoiceCommandLogs => Set<VoiceCommandLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostGIS eklentisini aktif et (Mekansal sorgular için şart)
        modelBuilder.HasPostgresExtension("postgis");

        // Tüm Entity konfigürasyonlarını otomatik tara ve uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MotoNavDbContext).Assembly);

        // Soft Delete (IsDeleted = false olanları otomatik filtrele)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    ConvertFilterExpression(entityType.ClrType)
                );
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Otomatik CreatedAt / UpdatedAt yönetimi
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }
}