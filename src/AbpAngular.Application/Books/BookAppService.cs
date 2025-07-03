using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbpAngular.Permissions;
using AbpAngular.Suppliers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;

namespace AbpAngular.Books;

[Authorize(AbpAngularPermissions.Books.Default)]
public class BookAppService : ApplicationService, IBookAppService
{
    private readonly IRepository<Book, Guid> _repository;
    private readonly IRepository<BookSupplier> _bookSupplierRepository;

    public BookAppService(
        IRepository<Book, Guid> repository,
        IRepository<BookSupplier> bookSupplierRepository)
    {
        _repository = repository;
        _bookSupplierRepository = bookSupplierRepository;
    }

    public async Task<BookDto> GetAsync(Guid id)
    {
        var queryable = await _repository.GetQueryableAsync();
        var book = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.Id == id)
        );
        
        if (book == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Book), id);
        }
        
        // Load suppliers for this book
        var bookSuppliers = await _bookSupplierRepository.GetListAsync(bs => bs.BookId == id);
        var supplierIds = bookSuppliers.Select(bs => bs.SupplierId).ToList();
        
        var bookDto = ObjectMapper.Map<Book, BookDto>(book);
        
        if (supplierIds.Any())
        {
            var supplierRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Supplier, Guid>>();
            var suppliers = await supplierRepository.GetListAsync(s => supplierIds.Contains(s.Id));
            bookDto.Suppliers = ObjectMapper.Map<List<Supplier>, List<SupplierDto>>(suppliers);
        }
        
        return bookDto;
    }    public async Task<PagedResultDto<BookDto>> GetListAsync(GetBookListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        
        // Apply filter if provided
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(input.Filter));
        }
        
        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Name" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var books = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<BookDto>(
            totalCount,
            ObjectMapper.Map<List<Book>, List<BookDto>>(books)
        );
    }

    [Authorize(AbpAngularPermissions.Books.Create)]
    public async Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        var book = ObjectMapper.Map<CreateUpdateBookDto, Book>(input);
        await _repository.InsertAsync(book);
        
        // Handle many-to-many relationship with suppliers
        if (input.SupplierIds?.Any() == true)
        {
            var bookSuppliers = input.SupplierIds.Select(supplierId => new BookSupplier
            {
                BookId = book.Id,
                SupplierId = supplierId
            }).ToList();
            
            await _bookSupplierRepository.InsertManyAsync(bookSuppliers);
        }
        
        return ObjectMapper.Map<Book, BookDto>(book);
    }

    [Authorize(AbpAngularPermissions.Books.Edit)]
    public async Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        var book = await _repository.GetAsync(id);
        ObjectMapper.Map(input, book);
        await _repository.UpdateAsync(book);
        
        // Remove existing supplier relationships
        var existingBookSuppliers = await _bookSupplierRepository.GetListAsync(bs => bs.BookId == id);
        await _bookSupplierRepository.DeleteManyAsync(existingBookSuppliers);
        
        // Add new supplier relationships
        if (input.SupplierIds?.Any() == true)
        {
            var bookSuppliers = input.SupplierIds.Select(supplierId => new BookSupplier
            {
                BookId = book.Id,
                SupplierId = supplierId
            }).ToList();
            
            await _bookSupplierRepository.InsertManyAsync(bookSuppliers);
        }
        
        return ObjectMapper.Map<Book, BookDto>(book);
    }

    [Authorize(AbpAngularPermissions.Books.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var book = await _repository.FindAsync(id);
        if (book == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Book), id);
        }
        await _repository.DeleteAsync(id);
    }
}
