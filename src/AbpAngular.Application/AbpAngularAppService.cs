using AbpAngular.Localization;
using Volo.Abp.Application.Services;

namespace AbpAngular;

/* Inherit your application services from this class.
 */
public abstract class AbpAngularAppService : ApplicationService
{
    protected AbpAngularAppService()
    {
        LocalizationResource = typeof(AbpAngularResource);
    }
}
