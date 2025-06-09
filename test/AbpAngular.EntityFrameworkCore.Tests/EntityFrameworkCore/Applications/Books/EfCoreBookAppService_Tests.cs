using AbpAngular.Books;
using Xunit;

namespace AbpAngular.EntityFrameworkCore.Applications.Books;

[Collection(AbpAngularTestConsts.CollectionDefinitionName)]
public class EfCoreBookAppService_Tests : BookAppService_Tests<AbpAngularEntityFrameworkCoreTestModule>
{

}