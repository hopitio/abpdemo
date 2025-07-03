import type { AuditedEntityDto } from '@abp/ng.core';
import type { BookType } from './book-type.enum';
import type { SupplierDto } from '../suppliers';

export interface BookDto extends AuditedEntityDto<string> {
  name?: string;
  type?: BookType;
  publishDate?: string;
  price: number;
  suppliers?: SupplierDto[];
}

export interface CreateUpdateBookDto {
  name: string;
  type: BookType;
  publishDate: string;
  price: number;
  supplierIds?: string[];
}
