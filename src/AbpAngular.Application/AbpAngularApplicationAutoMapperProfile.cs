using AutoMapper;
using AbpAngular.Books;

namespace AbpAngular;

public class AbpAngularApplicationAutoMapperProfile : Profile
{
    public AbpAngularApplicationAutoMapperProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<CreateUpdateBookDto, Book>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}
