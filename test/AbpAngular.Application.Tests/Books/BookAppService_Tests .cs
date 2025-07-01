using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace AbpAngular.Books;

public abstract class BookAppService_Tests<TStartupModule> : AbpAngularApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IBookAppService _bookAppService;

    protected BookAppService_Tests()
    {
        _bookAppService = GetRequiredService<IBookAppService>();
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
}