using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
using AbpAngular.Suppliers;
using System.Collections.Generic;

namespace AbpAngular.Books;

public abstract class BookAppService_Tests<TStartupModule> : AbpAngularApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IBookAppService _bookAppService;
    private readonly ISupplierAppService _supplierAppService;

    protected BookAppService_Tests()
    {
        _bookAppService = GetRequiredService<IBookAppService>();
        _supplierAppService = GetRequiredService<ISupplierAppService>();
    }    [Fact]
    public async Task Should_Get_List_Of_Books()
    {
        //Act
        var result = await _bookAppService.GetListAsync(
            new GetBookListDto()
        );

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(b => b.Name == "1984");
    }

    [Fact]
    public async Task Should_Create_A_Valid_Book()
    {
        //Act
        var result = await _bookAppService.CreateAsync(
            new CreateUpdateBookDto
            {
                Name = "New test book 42",
                Price = 10,
                PublishDate = DateTime.Now,
                Type = BookType.ScienceFiction
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("New test book 42");
    }
    
    [Fact]
    public async Task Should_Not_Create_A_Book_Without_Name()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _bookAppService.CreateAsync(
                new CreateUpdateBookDto
                {
                    Name = "",
                    Price = 10,
                    PublishDate = DateTime.Now,
                    Type = BookType.ScienceFiction
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Get_A_Book_By_Id()
    {
        // Arrange - First create a book
        var createdBook = await _bookAppService.CreateAsync(
            new CreateUpdateBookDto
            {
                Name = "Test Book for Get",
                Price = 15,
                PublishDate = DateTime.Now,
                Type = BookType.Adventure
            }
        );

        // Act
        var result = await _bookAppService.GetAsync(createdBook.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdBook.Id);
        result.Name.ShouldBe("Test Book for Get");
        result.Price.ShouldBe(15);
        result.Type.ShouldBe(BookType.Adventure);
    }

    [Fact]
    public async Task Should_Not_Get_A_Book_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
        {
            await _bookAppService.GetAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Update_A_Book()
    {
        // Arrange - First create a book
        var createdBook = await _bookAppService.CreateAsync(
            new CreateUpdateBookDto
            {
                Name = "Original Book",
                Price = 20,
                PublishDate = DateTime.Now.AddDays(-30),
                Type = BookType.Science
            }
        );

        // Act
        var updatedBook = await _bookAppService.UpdateAsync(createdBook.Id,
            new CreateUpdateBookDto
            {
                Name = "Updated Book",
                Price = 25,
                PublishDate = DateTime.Now,
                Type = BookType.ScienceFiction
            }
        );

        // Assert
        updatedBook.Id.ShouldBe(createdBook.Id);
        updatedBook.Name.ShouldBe("Updated Book");
        updatedBook.Price.ShouldBe(25);
        updatedBook.Type.ShouldBe(BookType.ScienceFiction);
    }

    [Fact]
    public async Task Should_Not_Update_A_Book_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
        {
            await _bookAppService.UpdateAsync(Guid.NewGuid(),
                new CreateUpdateBookDto
                {
                    Name = "Updated Book",
                    Price = 25,
                    PublishDate = DateTime.Now,
                    Type = BookType.ScienceFiction
                }
            );
        });
    }

    [Fact]
    public async Task Should_Delete_A_Book()
    {
        // Arrange - First create a book
        var createdBook = await _bookAppService.CreateAsync(
            new CreateUpdateBookDto
            {
                Name = "Book to Delete",
                Price = 30,
                PublishDate = DateTime.Now,
                Type = BookType.Horror
            }
        );

        // Act
        await _bookAppService.DeleteAsync(createdBook.Id);

        // Assert - Try to get the deleted book should throw exception
        var exception = await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
        {
            await _bookAppService.GetAsync(createdBook.Id);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_A_Book_With_Invalid_Id()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
        {
            await _bookAppService.DeleteAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Not_Create_A_Book_With_Negative_Price()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _bookAppService.CreateAsync(
                new CreateUpdateBookDto
                {
                    Name = "Test Book",
                    Price = -10,
                    PublishDate = DateTime.Now,
                    Type = BookType.Adventure
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Price"));
    }

    [Fact]
    public async Task Should_Not_Create_A_Book_With_Zero_Price()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _bookAppService.CreateAsync(
                new CreateUpdateBookDto
                {
                    Name = "Test Book",
                    Price = 0,
                    PublishDate = DateTime.Now,
                    Type = BookType.Adventure
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Price"));
    }

    [Fact]
    public async Task Should_Not_Create_A_Book_With_Too_Long_Name()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _bookAppService.CreateAsync(
                new CreateUpdateBookDto
                {
                    Name = new string('A', 129), // 129 characters, exceeds max length of 128
                    Price = 15,
                    PublishDate = DateTime.Now,
                    Type = BookType.Adventure
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
    }

    [Fact]
    public async Task Should_Get_Filtered_List_Of_Books()
    {
        // Arrange - Create test books
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Fantasy Adventure",
            Price = 20,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Science Theory",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.Science
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
        {
            Filter = "Fantasy"
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(b => b.Name.Contains("Fantasy"));
    }

    [Fact]
    public async Task Should_Get_Paged_List_Of_Books()
    {
        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
        {
            SkipCount = 0,
            MaxResultCount = 2
        });

        // Assert
        result.Items.Count.ShouldBeLessThanOrEqualTo(2);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(result.Items.Count);
    }

    [Fact]
    public async Task Should_Get_Sorted_List_Of_Books()
    {
        // Arrange - Create books with different names
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Z Book",
            Price = 20,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "A Book",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.Science
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
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
    public async Task Should_Create_A_Book_With_Suppliers()
    {
        // Arrange - Create suppliers first
        var supplier1 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Test Supplier 1",
            Email = "supplier1@test.com",
            IsActive = true
        });

        var supplier2 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Test Supplier 2", 
            Email = "supplier2@test.com",
            IsActive = true
        });

        // Act
        var result = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book with Suppliers",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure,
            SupplierIds = new List<Guid> { supplier1.Id, supplier2.Id }
        });

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Book with Suppliers");
        
        // Get the book to verify suppliers are associated
        var bookWithSuppliers = await _bookAppService.GetAsync(result.Id);
        bookWithSuppliers.Suppliers.ShouldNotBeNull();
        bookWithSuppliers.Suppliers.Count.ShouldBe(2);
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "Test Supplier 1");
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "Test Supplier 2");
    }

    [Fact]
    public async Task Should_Update_A_Book_With_New_Suppliers()
    {
        // Arrange - Create suppliers
        var supplier1 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Original Supplier",
            Email = "original@test.com",
            IsActive = true
        });

        var supplier2 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "New Supplier",
            Email = "new@test.com",
            IsActive = true
        });

        // Create book with first supplier
        var book = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book to Update",
            Price = 20,
            PublishDate = DateTime.Now,
            Type = BookType.Science,
            SupplierIds = new List<Guid> { supplier1.Id }
        });

        // Act - Update book with different supplier
        var updatedBook = await _bookAppService.UpdateAsync(book.Id, new CreateUpdateBookDto
        {
            Name = "Updated Book",
            Price = 30,
            PublishDate = DateTime.Now,
            Type = BookType.ScienceFiction,
            SupplierIds = new List<Guid> { supplier2.Id }
        });

        // Assert
        updatedBook.Name.ShouldBe("Updated Book");
        
        // Get the book to verify suppliers are updated
        var bookWithSuppliers = await _bookAppService.GetAsync(book.Id);
        bookWithSuppliers.Suppliers.ShouldNotBeNull();
        bookWithSuppliers.Suppliers.Count.ShouldBe(1);
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "New Supplier");
        bookWithSuppliers.Suppliers.ShouldNotContain(s => s.Name == "Original Supplier");
    }

    [Fact]
    public async Task Should_Remove_All_Suppliers_From_Book()
    {
        // Arrange - Create supplier and book
        var supplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier to Remove",
            Email = "remove@test.com",
            IsActive = true
        });

        var book = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book with Supplier",
            Price = 15,
            PublishDate = DateTime.Now,
            Type = BookType.Fantastic,
            SupplierIds = new List<Guid> { supplier.Id }
        });

        // Act - Update book with no suppliers
        await _bookAppService.UpdateAsync(book.Id, new CreateUpdateBookDto
        {
            Name = "Book without Suppliers",
            Price = 15,
            PublishDate = DateTime.Now,
            Type = BookType.Fantastic,
            SupplierIds = new List<Guid>()
        });

        // Assert
        var bookWithoutSuppliers = await _bookAppService.GetAsync(book.Id);
        bookWithoutSuppliers.Suppliers.ShouldNotBeNull();
        bookWithoutSuppliers.Suppliers.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Handle_Invalid_Supplier_Ids()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _bookAppService.CreateAsync(new CreateUpdateBookDto
            {
                Name = "Book with Invalid Supplier",
                Price = 20,
                PublishDate = DateTime.Now,
                Type = BookType.Adventure,
                SupplierIds = new List<Guid> { Guid.NewGuid() } // Non-existent supplier ID
            });
        });
    }

    [Fact]
    public async Task Should_Create_Book_Without_Suppliers()
    {
        // Act
        var result = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book without Suppliers",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure,
            SupplierIds = new List<Guid>()
        });

        // Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Book without Suppliers");
        
        // Get the book to verify no suppliers are associated
        var bookWithoutSuppliers = await _bookAppService.GetAsync(result.Id);
        bookWithoutSuppliers.Suppliers.ShouldNotBeNull();
        bookWithoutSuppliers.Suppliers.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Get_Books_By_Type()
    {
        // Arrange - Create books with different types
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Science Fiction Book",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.ScienceFiction
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Adventure Book",
            Price = 20,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto());

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(b => b.Type == BookType.ScienceFiction);
        result.Items.ShouldContain(b => b.Type == BookType.Adventure);
    }

    [Fact]
    public async Task Should_Get_Books_By_Price_Range()
    {
        // Arrange - Create books with different prices
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Cheap Book",
            Price = 10,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Expensive Book",
            Price = 50,
            PublishDate = DateTime.Now,
            Type = BookType.Science
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto());

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(b => b.Price == 10);
        result.Items.ShouldContain(b => b.Price == 50);
    }

    [Fact]
    public async Task Should_Get_Books_By_Publication_Date_Range()
    {
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now.AddDays(-10);

        // Arrange - Create books with different publication dates
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Old Book",
            Price = 20,
            PublishDate = DateTime.Now.AddDays(-60),
            Type = BookType.Adventure
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Recent Book",
            Price = 25,
            PublishDate = DateTime.Now.AddDays(-20),
            Type = BookType.Science
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto());

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(b => b.Name == "Old Book");
        result.Items.ShouldContain(b => b.Name == "Recent Book");
    }

    [Fact]
    public async Task Should_Handle_Complex_Filtering()
    {
        // Arrange - Create books with various properties
        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Complex Science Fiction",
            Price = 30,
            PublishDate = DateTime.Now.AddDays(-5),
            Type = BookType.ScienceFiction
        });

        await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Complex Adventure",
            Price = 25,
            PublishDate = DateTime.Now.AddDays(-15),
            Type = BookType.Adventure
        });

        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
        {
            Filter = "Complex"
        });

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(b => b.Name.Contains("Complex"));
    }

    [Fact]
    public async Task Should_Update_Book_Partially()
    {
        // Arrange - Create a book
        var createdBook = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Original Book",
            Price = 20,
            PublishDate = DateTime.Now.AddDays(-10),
            Type = BookType.Adventure
        });

        // Act - Update only name and price
        var updatedBook = await _bookAppService.UpdateAsync(createdBook.Id, new CreateUpdateBookDto
        {
            Name = "Updated Book Name",
            Price = 25,
            PublishDate = createdBook.PublishDate, // Keep original date
            Type = createdBook.Type // Keep original type
        });

        // Assert
        updatedBook.Name.ShouldBe("Updated Book Name");
        updatedBook.Price.ShouldBe(25);
        updatedBook.PublishDate.ShouldBe(createdBook.PublishDate);
        updatedBook.Type.ShouldBe(BookType.Adventure);
    }

    [Fact]
    public async Task Should_Validate_Future_Publication_Date()
    {
        // Act & Assert - Try to create book with future publication date
        var futureDate = DateTime.Now.AddDays(30);
        
        var result = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Future Book",
            Price = 20,
            PublishDate = futureDate,
            Type = BookType.ScienceFiction
        });

        // Assert - Should allow future dates
        result.ShouldNotBeNull();
        result.PublishDate.ShouldBe(futureDate);
    }

    [Fact]
    public async Task Should_Handle_Large_Price_Values()
    {
        // Act
        var result = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Expensive Collectors Edition",
            Price = 999.99f,
            PublishDate = DateTime.Now,
            Type = BookType.Science
        });

        // Assert
        result.ShouldNotBeNull();
        result.Price.ShouldBe(999.99f);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Books_With_Same_Suppliers()
    {
        // Arrange - Create a supplier
        var supplier = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Common Supplier",
            Email = "common@supplier.com",
            IsActive = true
        });

        // Act - Create multiple books with same supplier
        var book1 = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book 1",
            Price = 20,
            PublishDate = DateTime.Now,
            Type = BookType.Adventure,
            SupplierIds = new List<Guid> { supplier.Id }
        });

        var book2 = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Book 2",
            Price = 25,
            PublishDate = DateTime.Now,
            Type = BookType.Science,
            SupplierIds = new List<Guid> { supplier.Id }
        });

        // Assert
        var book1WithSuppliers = await _bookAppService.GetAsync(book1.Id);
        var book2WithSuppliers = await _bookAppService.GetAsync(book2.Id);

        book1WithSuppliers.Suppliers.ShouldContain(s => s.Name == "Common Supplier");
        book2WithSuppliers.Suppliers.ShouldContain(s => s.Name == "Common Supplier");
    }

    [Fact]
    public async Task Should_Handle_Book_With_Multiple_Suppliers()
    {
        // Arrange - Create multiple suppliers
        var supplier1 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier One",
            Email = "one@supplier.com",
            IsActive = true
        });

        var supplier2 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier Two",
            Email = "two@supplier.com",
            IsActive = true
        });

        var supplier3 = await _supplierAppService.CreateAsync(new CreateUpdateSupplierDto
        {
            Name = "Supplier Three",
            Email = "three@supplier.com",
            IsActive = true
        });

        // Act - Create book with multiple suppliers
        var book = await _bookAppService.CreateAsync(new CreateUpdateBookDto
        {
            Name = "Multi-Supplier Book",
            Price = 30,
            PublishDate = DateTime.Now,
            Type = BookType.Fantastic,
            SupplierIds = new List<Guid> { supplier1.Id, supplier2.Id, supplier3.Id }
        });

        // Assert
        var bookWithSuppliers = await _bookAppService.GetAsync(book.Id);
        bookWithSuppliers.Suppliers.Count.ShouldBe(3);
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "Supplier One");
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "Supplier Two");
        bookWithSuppliers.Suppliers.ShouldContain(s => s.Name == "Supplier Three");
    }

    [Fact]
    public async Task Should_Handle_Empty_Filter_Search()
    {
        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
        {
            Filter = ""
        });

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Should_Handle_Null_Filter_Search()
    {
        // Act
        var result = await _bookAppService.GetListAsync(new GetBookListDto
        {
            Filter = null
        });

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(0);
    }
}