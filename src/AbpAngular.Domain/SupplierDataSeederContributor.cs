using System;
using System.Threading.Tasks;
using AbpAngular.Suppliers;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AbpAngular;

public class SupplierDataSeederContributor
    : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Supplier, Guid> _supplierRepository;

    public SupplierDataSeederContributor(IRepository<Supplier, Guid> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _supplierRepository.GetCountAsync() <= 0)
        {
            await _supplierRepository.InsertAsync(
                new Supplier
                {
                    Name = "Penguin Random House",
                    Email = "info@penguinrandomhouse.com",
                    Phone = "+1-555-0123",
                    Address = "1745 Broadway, New York, NY 10019, USA",
                    Website = "https://www.penguinrandomhouse.com",
                    IsActive = true
                },
                autoSave: true
            );

            await _supplierRepository.InsertAsync(
                new Supplier
                {
                    Name = "HarperCollins Publishers",
                    Email = "info@harpercollins.com",
                    Phone = "+1-555-0456",
                    Address = "195 Broadway, New York, NY 10007, USA",
                    Website = "https://www.harpercollins.com",
                    IsActive = true
                },
                autoSave: true
            );

            await _supplierRepository.InsertAsync(
                new Supplier
                {
                    Name = "Macmillan Publishers",
                    Email = "info@macmillan.com",
                    Phone = "+1-555-0789",
                    Address = "120 Broadway, New York, NY 10271, USA",
                    Website = "https://www.macmillan.com",
                    IsActive = true
                },
                autoSave: true
            );
        }
    }
}
