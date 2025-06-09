using Volo.Abp.Modularity;

namespace AbpAngular;

[DependsOn(
    typeof(AbpAngularApplicationModule),
    typeof(AbpAngularDomainTestModule)
)]
public class AbpAngularApplicationTestModule : AbpModule
{

}
