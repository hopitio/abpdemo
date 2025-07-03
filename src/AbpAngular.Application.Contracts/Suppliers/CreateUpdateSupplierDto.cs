using System.ComponentModel.DataAnnotations;

namespace AbpAngular.Suppliers;

public class CreateUpdateSupplierDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;
    
    [OptionalEmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }
    
    [OptionalPhone]
    [StringLength(32)]
    public string? Phone { get; set; }
    
    [StringLength(512)]
    public string? Address { get; set; }
    
    [OptionalUrl]
    [StringLength(256)]
    public string? Website { get; set; }
    
    public bool IsActive { get; set; } = true;
}
