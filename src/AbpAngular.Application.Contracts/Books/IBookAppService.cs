﻿using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AbpAngular.Books;

public interface IBookAppService :
    ICrudAppService< //Defines CRUD methods
        BookDto, //Used to show books
        Guid, //Primary key of the book entity
        GetBookListDto, //Used for paging/sorting/filtering
        CreateUpdateBookDto> //Used to create/update a book
{

}