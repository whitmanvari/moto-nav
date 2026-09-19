using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MotoNav.Persistence.Context;

public class MotoNavDbContextFactory : IDesignTimeDbContextFactory<MotoNavDbContext>
{
    public MotoNavDbContext CreateDbContext(string[] args)
    {
        // 1. API projesinin yolunu bul
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../moto-nav.api");
        if (!Directory.Exists(basePath))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        // 2. secrets.json dosyasının Windows'taki doğrudan yolu
        var secretsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "UserSecrets", "1641880b-93a4-47c9-a4f3-1699f629c080", "secrets.json");

        // 3. Konfigürasyonu yükle
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile(secretsPath, optional: true)
            .Build();

        // 4. Bağlantı dizesini al
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("PostgresConnection")
            ?? configuration["ConnectionStrings:DefaultConnection"]
            ?? configuration["ConnectionStrings:PostgresConnection"];

        var optionsBuilder = new DbContextOptionsBuilder<MotoNavDbContext>();
        optionsBuilder.UseNpgsql(connectionString, x => x.UseNetTopologySuite());

        return new MotoNavDbContext(optionsBuilder.Options);
    }
}