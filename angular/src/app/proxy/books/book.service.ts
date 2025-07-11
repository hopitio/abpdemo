import type { BookDto, CreateUpdateBookDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BookService {
  apiName = 'Default';
  

  create = (input: CreateUpdateBookDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BookDto>({
      method: 'POST',
      url: '/api/app/book',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/book/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BookDto>({
      method: 'GET',
      url: `/api/app/book/${id}`,
    },
    { apiName: this.apiName,...config });
  
  getList = (input: PagedAndSortedResultRequestDto & { filter?: string }, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<BookDto>>({
      method: 'GET',
      url: '/api/app/book',
      params: { 
        sorting: input.sorting, 
        skipCount: input.skipCount, 
        maxResultCount: input.maxResultCount,
        filter: input.filter 
      },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateBookDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BookDto>({
      method: 'PUT',
      url: `/api/app/book/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
