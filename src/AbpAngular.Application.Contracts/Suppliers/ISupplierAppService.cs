using System;
using Volo.Abp.Application.Services;

namespace AbpAngular.Suppliers;

public interface ISupplierAppService :
    ICrudAppService<
        SupplierDto,
        Guid,
        GetSupplierListDto,
        CreateUpdateSupplierDto>
{
}
