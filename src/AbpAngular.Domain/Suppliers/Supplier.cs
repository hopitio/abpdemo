using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace AbpAngular.Suppliers;

public class Supplier : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Address { get; set; }
    
    public string? Website { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation property for many-to-many relationship
    public virtual ICollection<BookSupplier> BookSuppliers { get; set; } = new List<BookSupplier>();
}
