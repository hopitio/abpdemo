import { mapEnumToOptions } from '@abp/ng.core';

export enum SupplierStatus {
  Active = 1,
  Inactive = 0,
}

export const supplierStatusOptions = mapEnumToOptions(SupplierStatus);
