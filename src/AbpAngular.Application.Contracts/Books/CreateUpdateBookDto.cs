using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbpAngular.Books;

public class CreateUpdateBookDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public BookType Type { get; set; } = BookType.Undefined;

    [Required]
    [DataType(DataType.Date)]
    public DateTime PublishDate { get; set; } = DateTime.Now;

    [Required]
    [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public float Price { get; set; }
    
    public List<Guid> SupplierIds { get; set; } = new();
}