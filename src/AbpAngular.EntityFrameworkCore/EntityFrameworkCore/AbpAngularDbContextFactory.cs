using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AbpAngular.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class AbpAngularDbContextFactory : IDesignTimeDbContextFactory<AbpAngularDbContext>
{
    public AbpAngularDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        AbpAngularEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<AbpAngularDbContext>()
            .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);
        
        return new AbpAngularDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AbpAngular.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
