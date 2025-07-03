using Volo.Abp.Application.Dtos;

namespace AbpAngular.Suppliers;

public class GetSupplierListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    
    public bool? IsActive { get; set; }
}
