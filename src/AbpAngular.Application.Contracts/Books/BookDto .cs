using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using AbpAngular.Suppliers;

namespace AbpAngular.Books;

public class BookDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }
    
    public List<SupplierDto> Suppliers { get; set; } = new();
}