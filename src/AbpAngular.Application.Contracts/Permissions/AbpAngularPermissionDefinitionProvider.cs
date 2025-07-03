using AbpAngular.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace AbpAngular.Permissions;

public class AbpAngularPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AbpAngularPermissions.GroupName);

        var booksPermission = myGroup.AddPermission(AbpAngularPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(AbpAngularPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(AbpAngularPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(AbpAngularPermissions.Books.Delete, L("Permission:Books.Delete"));
        
        var suppliersPermission = myGroup.AddPermission(AbpAngularPermissions.Suppliers.Default, L("Permission:Suppliers"));
        suppliersPermission.AddChild(AbpAngularPermissions.Suppliers.Create, L("Permission:Suppliers.Create"));
        suppliersPermission.AddChild(AbpAngularPermissions.Suppliers.Edit, L("Permission:Suppliers.Edit"));
        suppliersPermission.AddChild(AbpAngularPermissions.Suppliers.Delete, L("Permission:Suppliers.Delete"));
        //Define your own permissions here. Example:
        //myGroup.AddPermission(AbpAngularPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpAngularResource>(name);
    }
}
