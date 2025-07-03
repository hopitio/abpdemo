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
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace AbpAngular.Books;

[Authorize(AbpAngularPermissions.Books.Default)]
public class BookAppService : ApplicationService, IBookAppService
{
    private readonly IRepository<Book, Guid> _repository;
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IDistributedCache<SupplierDto> _supplierCache;
    private readonly IMemoryCache _memoryCache;

    public BookAppService(
        IRepository<Book, Guid> repository,
        IRepository<Supplier, Guid> supplierRepository,
        IDistributedCache<SupplierDto> supplierCache,
        IMemoryCache memoryCache)
    {
        _repository = repository;
        _supplierRepository = supplierRepository;
        _supplierCache = supplierCache;
        _memoryCache = memoryCache;
    }

    // Helper methods for converting between string and list of IDs
    private List<Guid> ConvertStringToSupplierIds(string supplierIds)
    {
        return SupplierHelper.ConvertStringToSupplierIds(supplierIds);
    }

    private string ConvertSupplierIdsToString(List<Guid> supplierIds)
    {
        return SupplierHelper.ConvertSupplierIdsToString(supplierIds);
    }

    // Helper method to get suppliers with caching
    private async Task<List<SupplierDto>> GetSuppliersWithCacheAsync(List<Guid> supplierIds)
    {
        if (!supplierIds.Any())
        {
            return new List<SupplierDto>();
        }

        var suppliers = new List<SupplierDto>();
        var uncachedIds = new List<Guid>();

        // Check cache first
        foreach (var supplierId in supplierIds)
        {
            var cacheKey = $"supplier_{supplierId}";
            var cachedSupplier = _memoryCache.Get<SupplierDto>(cacheKey);
            
            if (cachedSupplier != null)
            {
                suppliers.Add(cachedSupplier);
            }
            else
            {
                uncachedIds.Add(supplierId);
            }
        }

        // Fetch uncached suppliers from database
        if (uncachedIds.Any())
        {
            var dbSuppliers = await _supplierRepository.GetListAsync(s => uncachedIds.Contains(s.Id));
            var dbSupplierDtos = ObjectMapper.Map<List<Supplier>, List<SupplierDto>>(dbSuppliers);

            // Cache the fetched suppliers
            foreach (var supplierDto in dbSupplierDtos)
            {
                var cacheKey = $"supplier_{supplierDto.Id}";
                _memoryCache.Set(cacheKey, supplierDto, TimeSpan.FromMinutes(15)); // Cache for 15 minutes
                suppliers.Add(supplierDto);
            }
        }

        return suppliers;
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
        
        var bookDto = ObjectMapper.Map<Book, BookDto>(book);
        
        // Load suppliers from the Suppliers field
        var supplierIds = ConvertStringToSupplierIds(book.Suppliers);
        bookDto.Suppliers = await GetSuppliersWithCacheAsync(supplierIds);
        
        return bookDto;
    }

    public async Task<PagedResultDto<BookDto>> GetListAsync(GetBookListDto input)
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

        var bookDtos = ObjectMapper.Map<List<Book>, List<BookDto>>(books);

        // Load suppliers for each book from the Suppliers field
        var allSupplierIds = new List<Guid>();
        var bookSupplierMapping = new Dictionary<Guid, List<Guid>>();

        foreach (var book in books)
        {
            var supplierIds = ConvertStringToSupplierIds(book.Suppliers);
            bookSupplierMapping[book.Id] = supplierIds;
            allSupplierIds.AddRange(supplierIds);
        }

        // Get unique supplier IDs and fetch them with cache
        var uniqueSupplierIds = allSupplierIds.Distinct().ToList();
        var allSuppliers = await GetSuppliersWithCacheAsync(uniqueSupplierIds);
        var supplierDictionary = allSuppliers.ToDictionary(s => s.Id);

        // Assign suppliers to each book
        foreach (var bookDto in bookDtos)
        {
            var bookId = bookDto.Id;
            if (bookSupplierMapping.ContainsKey(bookId))
            {
                var bookSupplierIds = bookSupplierMapping[bookId];
                bookDto.Suppliers = bookSupplierIds
                    .Where(id => supplierDictionary.ContainsKey(id))
                    .Select(id => supplierDictionary[id])
                    .ToList();
            }
            else
            {
                bookDto.Suppliers = new List<SupplierDto>();
            }
        }

        return new PagedResultDto<BookDto>(
            totalCount,
            bookDtos
        );
    }

    [Authorize(AbpAngularPermissions.Books.Create)]
    public async Task<BookDto> CreateAsync(CreateUpdateBookDto input)
    {
        var book = ObjectMapper.Map<CreateUpdateBookDto, Book>(input);
        
        // Handle comma-separated supplier IDs
        var supplierIds = ConvertStringToSupplierIds(input.Suppliers);
        if (input.SupplierIds?.Any() == true)
        {
            supplierIds.AddRange(input.SupplierIds);
        }
        
        // Store as comma-separated string in the book entity
        book.Suppliers = ConvertSupplierIdsToString(supplierIds.Distinct().ToList());
        
        await _repository.InsertAsync(book);
        
        var bookDto = ObjectMapper.Map<Book, BookDto>(book);
        
        // Load suppliers for the response
        bookDto.Suppliers = await GetSuppliersWithCacheAsync(supplierIds.Distinct().ToList());
        
        return bookDto;
    }

    [Authorize(AbpAngularPermissions.Books.Edit)]
    public async Task<BookDto> UpdateAsync(Guid id, CreateUpdateBookDto input)
    {
        var book = await _repository.GetAsync(id);
        ObjectMapper.Map(input, book);
        
        // Handle comma-separated supplier IDs
        var supplierIds = ConvertStringToSupplierIds(input.Suppliers);
        if (input.SupplierIds?.Any() == true)
        {
            supplierIds.AddRange(input.SupplierIds);
        }
        
        // Store as comma-separated string in the book entity
        book.Suppliers = ConvertSupplierIdsToString(supplierIds.Distinct().ToList());
        
        await _repository.UpdateAsync(book);
        
        var bookDto = ObjectMapper.Map<Book, BookDto>(book);
        
        // Load suppliers for the response
        bookDto.Suppliers = await GetSuppliersWithCacheAsync(supplierIds.Distinct().ToList());
        
        return bookDto;
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
