using Volo.Abp.Modularity;

namespace AbpAngular;

/* Inherit from this class for your domain layer tests. */
public abstract class AbpAngularDomainTestBase<TStartupModule> : AbpAngularTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
