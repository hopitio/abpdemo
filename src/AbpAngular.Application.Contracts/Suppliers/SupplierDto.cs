using System;
using Volo.Abp.Application.Dtos;

namespace AbpAngular.Suppliers;

public class SupplierDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Address { get; set; }
    
    public string? Website { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreationTime { get; set; }
}
