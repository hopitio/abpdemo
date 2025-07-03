using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using AbpAngular.Suppliers;

namespace AbpAngular.Books;

public class Book : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }
    
    // Comma-separated supplier IDs
    public string Suppliers { get; set; } = string.Empty;
    
    // Navigation property for many-to-many relationship
    public virtual ICollection<BookSupplier> BookSuppliers { get; set; } = new List<BookSupplier>();
}