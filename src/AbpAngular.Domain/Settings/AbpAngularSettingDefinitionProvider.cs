using Volo.Abp.Settings;

namespace AbpAngular.Settings;

public class AbpAngularSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AbpAngularSettings.MySetting1));
    }
}
