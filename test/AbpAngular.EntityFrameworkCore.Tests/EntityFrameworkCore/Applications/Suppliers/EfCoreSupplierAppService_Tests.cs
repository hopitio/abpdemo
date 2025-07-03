using AbpAngular.Suppliers;
using Xunit;

namespace AbpAngular.EntityFrameworkCore.Applications.Suppliers;

[Collection(AbpAngularTestConsts.CollectionDefinitionName)]
public class EfCoreSupplierAppService_Tests : SupplierAppService_Tests<AbpAngularEntityFrameworkCoreTestModule>
{

}
