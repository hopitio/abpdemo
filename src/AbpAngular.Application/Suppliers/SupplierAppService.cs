using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbpAngular.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;

namespace AbpAngular.Suppliers;

[Authorize(AbpAngularPermissions.Suppliers.Default)]
public class SupplierAppService : ApplicationService, ISupplierAppService
{
    private readonly IRepository<Supplier, Guid> _repository;

    public SupplierAppService(IRepository<Supplier, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto> GetAsync([Required] Guid id)
    {
        var supplier = await _repository.GetAsync(id);
        return ObjectMapper.Map<Supplier, SupplierDto>(supplier);
    }

    public async Task<PagedResultDto<SupplierDto>> GetListAsync(GetSupplierListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        
        // Apply filter if provided
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(input.Filter) || 
                                           (x.Email != null && x.Email.Contains(input.Filter)));
        }
        
        // Apply IsActive filter if provided
        if (input.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        }
        
        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Name" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var suppliers = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<SupplierDto>(
            totalCount,
            ObjectMapper.Map<List<Supplier>, List<SupplierDto>>(suppliers)
        );
    }

    [Authorize(AbpAngularPermissions.Suppliers.Create)]
    public async Task<SupplierDto> CreateAsync(CreateUpdateSupplierDto input)
    {
        var supplier = ObjectMapper.Map<CreateUpdateSupplierDto, Supplier>(input);
        await _repository.InsertAsync(supplier);
        return ObjectMapper.Map<Supplier, SupplierDto>(supplier);
    }

    [Authorize(AbpAngularPermissions.Suppliers.Edit)]
    public async Task<SupplierDto> UpdateAsync([Required] Guid id, CreateUpdateSupplierDto input)
    {
        var supplier = await _repository.GetAsync(id);
        ObjectMapper.Map(input, supplier);
        await _repository.UpdateAsync(supplier);
        return ObjectMapper.Map<Supplier, SupplierDto>(supplier);
    }

    [Authorize(AbpAngularPermissions.Suppliers.Delete)]
    public async Task DeleteAsync([Required] Guid id)
    {
        var supplier = await _repository.FindAsync(id);
        if (supplier == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Supplier), id);
        }
        await _repository.DeleteAsync(id);
    }
}
