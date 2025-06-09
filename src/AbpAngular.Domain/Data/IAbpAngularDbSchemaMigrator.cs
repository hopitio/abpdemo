using System.Threading.Tasks;

namespace AbpAngular.Data;

public interface IAbpAngularDbSchemaMigrator
{
    Task MigrateAsync();
}
