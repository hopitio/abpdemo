using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AbpAngular.Data;

/* This is used if database provider does't define
 * IAbpAngularDbSchemaMigrator implementation.
 */
public class NullAbpAngularDbSchemaMigrator : IAbpAngularDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
