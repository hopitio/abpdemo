using AutoMapper;
using AbpAngular.Books;
using AbpAngular.Suppliers;
using System.Linq;

namespace AbpAngular;

public class AbpAngularApplicationAutoMapperProfile : Profile
{
    public AbpAngularApplicationAutoMapperProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src => src.BookSuppliers.Select(bs => bs.Supplier)));
        CreateMap<CreateUpdateBookDto, Book>()
            .ForMember(dest => dest.BookSuppliers, opt => opt.Ignore());
        
        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateUpdateSupplierDto, Supplier>();
        
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}
