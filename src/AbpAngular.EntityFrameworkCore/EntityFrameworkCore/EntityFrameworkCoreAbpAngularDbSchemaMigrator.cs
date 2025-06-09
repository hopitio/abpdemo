using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AbpAngular.Data;
using Volo.Abp.DependencyInjection;

namespace AbpAngular.EntityFrameworkCore;

public class EntityFrameworkCoreAbpAngularDbSchemaMigrator
    : IAbpAngularDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreAbpAngularDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the AbpAngularDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<AbpAngularDbContext>()
            .Database
            .MigrateAsync();
    }
}
