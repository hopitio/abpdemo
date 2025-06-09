using AbpAngular.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AbpAngular.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAngularEntityFrameworkCoreModule),
    typeof(AbpAngularApplicationContractsModule)
)]
public class AbpAngularDbMigratorModule : AbpModule
{
}
