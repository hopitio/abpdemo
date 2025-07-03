using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace AbpAngular.Suppliers;

public abstract class SupplierAppService_Tests<TStartupModule> : AbpAngularApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ISupplierAppService _supplierAppService;

    protected SupplierAppService_Tests()
    {
        _supplierAppService = GetRequiredService<ISupplierAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_Suppliers()
    {
        // Arrange - Create a test supplier
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Test Supplier for List",
            Email = "testlist@supplier.com",
            Phone = "123-456-7890",
            Address = "Test Address",
            Website = "www.testsupplier.com",
            IsActive = true
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto());

        // Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(s => s.Name == "Test Supplier for List");
    }

    [Fact]
    public async Task Should_Create_A_Valid_Supplier()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "New Test Supplier",
            Email = "newtest@supplier.com",
            Phone = "987-654-3210",
            Address = "123 Test Street",
            Website = "www.newtestsupplier.com",
            IsActive = true
        });

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New Test Supplier");
        result.Email.ShouldBe("newtest@supplier.com");
        result.Phone.ShouldBe("987-654-3210");
        result.Address.ShouldBe("123 Test Street");
        result.Website.ShouldBe("www.newtestsupplier.com");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Create_A_Supplier_Without_Name()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = "",
                Email = "test@supplier.com",
                IsActive = true
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Not_Create_A_Supplier_Without_Email()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = "Test Supplier",
                Email = "",
                IsActive = true
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Email"));
    }

    [Fact]
    public async Task Should_Not_Create_A_Supplier_With_Invalid_Email()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = "Test Supplier",
                Email = "invalid-email",
                IsActive = true
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Email"));
    }

    [Fact]
    public async Task Should_Get_A_Supplier_By_Id()
    {
        // Arrange - First create a supplier
        var createdSupplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Test Supplier for Get",
            Email = "gettest@supplier.com",
            Phone = "555-123-4567",
            Address = "456 Get Test Ave",
            Website = "www.gettest.com",
            IsActive = true
        });

        // Act
        var result = await _supplierAppService.GetAsync(createdSupplier.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdSupplier.Id);
        result.Name.ShouldBe("Test Supplier for Get");
        result.Email.ShouldBe("gettest@supplier.com");
        result.Phone.ShouldBe("555-123-4567");
        result.Address.ShouldBe("456 Get Test Ave");
        result.Website.ShouldBe("www.gettest.com");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Get_A_Supplier_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _supplierAppService.GetAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Update_A_Supplier()
    {
        // Arrange - First create a supplier
        var createdSupplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Original Supplier",
            Email = "original@supplier.com",
            Phone = "111-222-3333",
            Address = "Original Address",
            Website = "www.original.com",
            IsActive = true
        });

        // Act
        var updatedSupplier = await _supplierAppService.UpdateAsync(createdSupplier.Id, new CreateUpdateSupplierDto
        {
            Name = "Updated Supplier",
            Email = "updated@supplier.com",
            Phone = "444-555-6666",
            Address = "Updated Address",
            Website = "www.updated.com",
            IsActive = false
        });

        // Assert
        updatedSupplier.Id.ShouldBe(createdSupplier.Id);
        updatedSupplier.Name.ShouldBe("Updated Supplier");
        updatedSupplier.Email.ShouldBe("updated@supplier.com");
        updatedSupplier.Phone.ShouldBe("444-555-6666");
        updatedSupplier.Address.ShouldBe("Updated Address");
        updatedSupplier.Website.ShouldBe("www.updated.com");
        updatedSupplier.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Not_Update_A_Supplier_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _supplierAppService.UpdateAsync(Guid.NewGuid(), new CreateUpdateSupplierDto
            {
                Name = "Updated Supplier",
                Email = "updated@supplier.com",
                IsActive = true
            });
        });
    }

    [Fact]
    public async Task Should_Delete_A_Supplier()
    {
        // Arrange - First create a supplier
        var createdSupplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier to Delete",
            Email = "delete@supplier.com",
            IsActive = true
        });

        // Act
        await _supplierAppService.DeleteAsync(createdSupplier.Id);

        // Assert - Try to get the deleted supplier should throw exception
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _supplierAppService.GetAsync(createdSupplier.Id);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_A_Supplier_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _supplierAppService.DeleteAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Get_Filtered_List_Of_Suppliers()
    {
        // Arrange - Create test suppliers
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "ABC Tech Supplier",
            Email = "abc@tech.com",
            IsActive = true
        });

        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "XYZ Books Supplier",
            Email = "xyz@books.com",
            IsActive = true
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            Filter = "ABC"
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(s => s.Name.Contains("ABC"));
    }

    [Fact]
    public async Task Should_Get_Active_Suppliers_Only()
    {
        // Arrange - Create active and inactive suppliers
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Active Supplier",
            Email = "active@supplier.com",
            IsActive = true
        });

        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Inactive Supplier",
            Email = "inactive@supplier.com",
            IsActive = false
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            IsActive = true
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(s => s.IsActive);
    }

    [Fact]
    public async Task Should_Get_Paged_List_Of_Suppliers()
    {
        // Arrange - Create multiple suppliers
        for (int i = 0; i < 5; i++)
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = $"Supplier {i}",
                Email = $"supplier{i}@test.com",
                IsActive = true
            });
        }

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            SkipCount = 0,
            MaxResultCount = 3
        });

        // Assert
        result.Items.Count.ShouldBeLessThanOrEqualTo(3);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(result.Items.Count);
    }

    [Fact]
    public async Task Should_Get_Sorted_List_Of_Suppliers()
    {
        // Arrange - Create suppliers with different names
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Zebra Supplier",
            Email = "zebra@supplier.com",
            IsActive = true
        });

        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Alpha Supplier",
            Email = "alpha@supplier.com",
            IsActive = true
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            Sorting = "Name desc"
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        if (result.Items.Count > 1)
        {
            for (int i = 0; i < result.Items.Count - 1; i++)
            {
                string.Compare(result.Items[i].Name, result.Items[i + 1].Name, StringComparison.OrdinalIgnoreCase)
                    .ShouldBeGreaterThanOrEqualTo(0);
            }
        }
    }

    [Fact]
    public async Task Should_Not_Create_A_Supplier_With_Too_Long_Name()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = new string('A', 129), // 129 characters, exceeds max length of 128
                Email = "test@supplier.com",
                IsActive = true
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Not_Create_A_Supplier_With_Too_Long_Email()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = "Test Supplier",
                Email = new string('a', 250) + "@test.com", // Very long email
                IsActive = true
            });
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Email"));
    }

    [Fact]
    public async Task Should_Create_A_Supplier_With_Minimal_Required_Fields()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Minimal Supplier",
            Email = "minimal@supplier.com",
            IsActive = true
            // Phone, Address, Website are optional
        });

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Minimal Supplier");
        result.Email.ShouldBe("minimal@supplier.com");
        result.IsActive.ShouldBeTrue();
        result.Phone.ShouldBeNullOrEmpty();
        result.Address.ShouldBeNullOrEmpty();
        result.Website.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Create_Supplier_With_Duplicate_Email()
    {
        // Arrange - Create first supplier
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "First Supplier",
            Email = "duplicate@supplier.com",
            IsActive = true
        });

        // Act - Create second supplier with same email (should succeed as there's no unique constraint)
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Second Supplier",
            Email = "duplicate@supplier.com",
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Second Supplier");
        result.Email.ShouldBe("duplicate@supplier.com");
    }

    [Fact]
    public async Task Should_Get_Inactive_Suppliers_Only()
    {
        // Arrange - Create active and inactive suppliers
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Active Supplier Test",
            Email = "active_test@supplier.com",
            IsActive = true
        });

        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Inactive Supplier Test",
            Email = "inactive_test@supplier.com",
            IsActive = false
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            IsActive = false
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(s => !s.IsActive);
    }

    [Fact]
    public async Task Should_Update_Supplier_Status()
    {
        // Arrange - Create active supplier
        var createdSupplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Status Test Supplier",
            Email = "status@supplier.com",
            IsActive = true
        });

        // Act - Update to inactive
        var updatedSupplier = await _supplierAppService.UpdateAsync(createdSupplier.Id, new CreateUpdateSupplierDto
        {
            Name = "Status Test Supplier",
            Email = "status@supplier.com",
            IsActive = false
        });

        // Assert
        updatedSupplier.IsActive.ShouldBeFalse();
        
        // Verify in database
        var retrievedSupplier = await _supplierAppService.GetAsync(createdSupplier.Id);
        retrievedSupplier.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Handle_Supplier_With_Special_Characters_In_Name()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier & Co. (Ltd.) - Special Characters!",
            Email = "special@supplier.com",
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Supplier & Co. (Ltd.) - Special Characters!");
    }

    [Fact]
    public async Task Should_Handle_Supplier_With_International_Characters()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Fournisseur Éléctronique Français",
            Email = "international@supplier.com",
            Phone = "+33-1-2345-6789",
            Address = "123 Rue de la Paix, Paris, France",
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Fournisseur Éléctronique Français");
        result.Phone.ShouldBe("+33-1-2345-6789");
        result.Address.ShouldBe("123 Rue de la Paix, Paris, France");
    }

    [Fact]
    public async Task Should_Handle_Supplier_With_Long_Website_URL()
    {
        // Act
        var longWebsite = "https://www.very-long-domain-name-for-testing-purposes.supplier-company.international.com/products/category/subcategory";
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Long Website Supplier",
            Email = "longwebsite@supplier.com",
            Website = longWebsite,
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Website.ShouldBe(longWebsite);
    }

    [Fact]
    public async Task Should_Handle_Supplier_With_Complete_Information()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Complete Information Supplier",
            Email = "complete@supplier.com",
            Phone = "+1-555-123-4567",
            Address = "123 Business Park, Suite 456, Commercial District, Metro City, State 12345",
            Website = "https://www.complete-supplier.com",
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Complete Information Supplier");
        result.Email.ShouldBe("complete@supplier.com");
        result.Phone.ShouldBe("+1-555-123-4567");
        result.Address.ShouldBe("123 Business Park, Suite 456, Commercial District, Metro City, State 12345");
        result.Website.ShouldBe("https://www.complete-supplier.com");
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Filter_Suppliers_By_Email_Domain()
    {
        // Arrange - Create suppliers with different email domains
        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Gmail Supplier",
            Email = "supplier1@gmail.com",
            IsActive = true
        });

        await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Company Supplier",
            Email = "supplier2@company.com",
            IsActive = true
        });

        // Act
        var result = await _supplierAppService.GetListAsync(new GetSupplierListDto
        {
            Filter = "gmail"
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(s => s.Email != null && s.Email.Contains("gmail"));
    }

    [Fact]
    public async Task Should_Handle_Supplier_Bulk_Operations()
    {
        // Arrange - Create multiple suppliers
        var suppliers = new List<CreateUpdateSupplierDto>();
        for (int i = 1; i <= 10; i++)
        {
            suppliers.Add(new CreateUpdateSupplierDto
            {
                Name = $"Bulk Supplier {i}",
                Email = $"bulk{i}@supplier.com",
                IsActive = i % 2 == 0 // Even numbers are active
            });
        }

        // Act - Create all suppliers
        var createdSuppliers = new List<SupplierDto>();
        foreach (var supplier in suppliers)
        {
            var created = await _supplierAppService.CreateAsync(supplier);
            createdSuppliers.Add(created);
        }

        // Assert
        createdSuppliers.Count.ShouldBe(10);
        createdSuppliers.Count(s => s.IsActive).ShouldBe(5);
        createdSuppliers.Count(s => !s.IsActive).ShouldBe(5);
    }

    [Fact]
    public async Task Should_Handle_Supplier_With_Empty_Optional_Fields()
    {
        // Act
        var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Basic Supplier",
            Email = "basic@supplier.com",
            Phone = "",
            Address = "",
            Website = "",
            IsActive = true
        });

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Basic Supplier");
        result.Email.ShouldBe("basic@supplier.com");
        result.Phone.ShouldBeNullOrEmpty();
        result.Address.ShouldBeNullOrEmpty();
        result.Website.ShouldBeNullOrEmpty();
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Handle_Supplier_Phone_Number_Formats()
    {
        // Arrange - Various phone number formats
        var phoneFormats = new[]
        {
            "+1-555-123-4567",
            "(555) 123-4567",
            "555.123.4567",
            "5551234567",
            "+44 20 7123 4567",
            "+86 138 0013 8000"
        };

        // Act & Assert
        for (int i = 0; i < phoneFormats.Length; i++)
        {
            var result = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
            {
                Name = $"Phone Format Supplier {i + 1}",
                Email = $"phone{i + 1}@supplier.com",
                Phone = phoneFormats[i],
                IsActive = true
            });

            result.ShouldNotBeNull();
            result.Phone.ShouldBe(phoneFormats[i]);
        }
    }

    [Fact]
    public async Task Should_Handle_Concurrent_Supplier_Creation()
    {
        // Arrange
        var tasks = new List<Task<SupplierDto>>();
        
        // Act - Create multiple suppliers concurrently
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            tasks.Add(Task.Run(async () => 
                await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
                {
                    Name = $"Concurrent Supplier {index}",
                    Email = $"concurrent{index}@supplier.com",
                    IsActive = true
                })
            ));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Length.ShouldBe(5);
        results.ShouldAllBe(r => r != null);
        results.Select(r => r.Name).ShouldAllBe(name => name.StartsWith("Concurrent Supplier"));
    }
}
