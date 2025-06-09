using Microsoft.Extensions.Localization;
using AbpAngular.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AbpAngular;

[Dependency(ReplaceServices = true)]
public class AbpAngularBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AbpAngularResource> _localizer;

    public AbpAngularBrandingProvider(IStringLocalizer<AbpAngularResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
