import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { SupplierService } from './shared/supplier.service';
import { SupplierDto, CreateUpdateSupplierDto, GetSupplierListDto } from './shared/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { take } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-supplier',
  templateUrl: './supplier.component.html',
  styleUrls: ['./supplier.component.scss'],
  providers: [ListService],
})
export class SupplierComponent implements OnInit {
  supplier = { items: [], totalCount: 0 } as PagedResultDto<SupplierDto>;

  selectedSupplier = {} as SupplierDto;

  form: FormGroup;

  isModalOpen = false;

  constructor(
    public readonly list: ListService,
    private supplierService: SupplierService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit() {
    const supplierStreamCreator = (query) => this.supplierService.getList(query);

    this.list.hookToQuery(supplierStreamCreator).subscribe((response) => {
      this.supplier = response;
    });
  }

  createSupplier() {
    this.selectedSupplier = {} as SupplierDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editSupplier(id: string) {
    this.supplierService.get(id).subscribe((supplier) => {
      this.selectedSupplier = supplier;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      name: [this.selectedSupplier.name || '', Validators.required],
      email: [this.selectedSupplier.email || '', [this.optionalEmailValidator]],
      phone: [this.selectedSupplier.phone || '', [this.optionalPhoneValidator]],
      address: [this.selectedSupplier.address || ''],
      website: [this.selectedSupplier.website || '', [this.optionalWebsiteValidator]],
      isActive: [this.selectedSupplier.isActive !== undefined ? this.selectedSupplier.isActive : true],
    });
  }

  // Custom validator for optional email field
  optionalEmailValidator(control: any) {
    if (!control.value || control.value.trim() === '') {
      return null; // Valid if empty
    }
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(control.value) ? null : { email: true };
  }

  // Custom validator for optional phone field
  optionalPhoneValidator(control: any) {
    if (!control.value || control.value.trim() === '') {
      return null; // Valid if empty
    }
    const phonePattern = /^[\+]?[0-9\s\-\(\)]{7,20}$/;
    return phonePattern.test(control.value) ? null : { phone: true };
  }

  // Custom validator for optional website field
  optionalWebsiteValidator(control: any) {
    if (!control.value || control.value.trim() === '') {
      return null; // Valid if empty
    }
    try {
      const url = new URL(control.value);
      const validSchemes = ['http:', 'https:', 'ftp:'];
      return validSchemes.includes(url.protocol) ? null : { url: true };
    } catch {
      return { url: true };
    }
  }

  save() {
    if (this.form.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      return;
    }

    const request = this.selectedSupplier.id
      ? this.supplierService.update(this.selectedSupplier.id, this.form.value)
      : this.supplierService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSure', '::AreYouSureToDelete').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.supplierService.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  onSearch(value: string) {
    this.list.filter = value;
    this.list.get();
  }

  onActiveFilter(isActive: boolean | null) {
    // Create custom query parameters and update directly
    const query: GetSupplierListDto = {
      filter: this.list.filter || '',
      isActive: isActive,
      sorting: 'name',
      skipCount: 0,
      maxResultCount: 10
    };
    
    this.supplierService.getList(query).subscribe((response) => {
      this.supplier = response;
    });
  }
}
