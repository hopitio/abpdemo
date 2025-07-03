using System;
using Volo.Abp.Domain.Entities;
using AbpAngular.Books;

namespace AbpAngular.Suppliers;

public class BookSupplier : Entity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; }
    
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    
    public DateTime CreationTime { get; set; } = DateTime.Now;
    
    public override object[] GetKeys()
    {
        return new object[] { BookId, SupplierId };
    }
}
