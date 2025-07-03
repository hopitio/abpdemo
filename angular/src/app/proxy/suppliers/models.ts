import type { AuditedEntityDto, EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface SupplierDto extends EntityDto<string> {
  name: string;
  email?: string;
  phone?: string;
  address?: string;
  website?: string;
  isActive: boolean;
  creationTime: string;
}

export interface CreateUpdateSupplierDto {
  name: string;
  email?: string;
  phone?: string;
  address?: string;
  website?: string;
  isActive: boolean;
}

export interface GetSupplierListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  isActive?: boolean;
}
