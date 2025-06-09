using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace AbpAngular.Books;

public class GetBookListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
