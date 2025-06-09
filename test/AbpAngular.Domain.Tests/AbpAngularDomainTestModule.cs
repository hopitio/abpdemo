using Volo.Abp.Modularity;

namespace AbpAngular;

[DependsOn(
    typeof(AbpAngularDomainModule),
    typeof(AbpAngularTestBaseModule)
)]
public class AbpAngularDomainTestModule : AbpModule
{

}
