using Volo.Abp.Modularity;

namespace AbpAngular;

public abstract class AbpAngularApplicationTestBase<TStartupModule> : AbpAngularTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
