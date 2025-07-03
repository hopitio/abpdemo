import type { CreateUpdateSupplierDto, GetSupplierListDto, SupplierDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SupplierService {
  apiName = 'Default';
  
  constructor(private restService: RestService) {}

  create = (input: CreateUpdateSupplierDto) =>
    this.restService.request<any, SupplierDto>({
      method: 'POST',
      url: '/api/app/supplier',
      body: input,
    },
    { apiName: this.apiName });

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/supplier/${id}`,
    },
    { apiName: this.apiName });

  get = (id: string) =>
    this.restService.request<any, SupplierDto>({
      method: 'GET',
      url: `/api/app/supplier/${id}`,
    },
    { apiName: this.apiName });

  getList = (input: GetSupplierListDto) =>
    this.restService.request<any, PagedResultDto<SupplierDto>>({
      method: 'GET',
      url: '/api/app/supplier',
      params: { 
        filter: input.filter, 
        isActive: input.isActive,
        sorting: input.sorting, 
        skipCount: input.skipCount, 
        maxResultCount: input.maxResultCount 
      },
    },
    { apiName: this.apiName });

  update = (id: string, input: CreateUpdateSupplierDto) =>
    this.restService.request<any, SupplierDto>({
      method: 'PUT',
      url: `/api/app/supplier/${id}`,
      body: input,
    },
    { apiName: this.apiName });
}
