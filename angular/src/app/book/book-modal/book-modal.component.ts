import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { BookService, BookDto, bookTypeOptions } from '../../proxy/books';
import { SupplierService, SupplierDto } from '../../proxy/suppliers';

@Component({
  standalone: false,
  selector: 'app-book-modal',
  templateUrl: './book-modal.component.html',
  styleUrls: ['./book-modal.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class BookModalComponent implements OnChanges, OnInit {
  @Input() isModalOpen = false;
  @Input() selectedBook = {} as BookDto;
  @Output() isModalOpenChange = new EventEmitter<boolean>();
  @Output() onSave = new EventEmitter<void>();

  form: FormGroup;
  bookTypes = bookTypeOptions;
  activeTab = 'basic'; // Default tab
  suppliers: SupplierDto[] = [];

  constructor(
    private bookService: BookService,
    private supplierService: SupplierService,
    private fb: FormBuilder
  ) {
    this.buildForm();
  }

  ngOnInit() {
    this.loadSuppliers();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['selectedBook'] && changes['selectedBook'].currentValue) {
      this.buildForm();
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: [this.selectedBook.name || '', Validators.required],
      type: [this.selectedBook.type || null, Validators.required],
      publishDate: [
        this.selectedBook.publishDate ? new Date(this.selectedBook.publishDate) : null,
        Validators.required,
      ],
      price: [this.selectedBook.price || null, Validators.required],
      supplierIds: [this.selectedBook.suppliers?.map(s => s.id) || []],
    });
  }

  loadSuppliers() {
    this.supplierService.getList({
      filter: '',
      isActive: true,
      sorting: 'name',
      skipCount: 0,
      maxResultCount: 1000
    }).subscribe(result => {
      this.suppliers = result.items;
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedBook.id
      ? this.bookService.update(this.selectedBook.id, this.form.value)
      : this.bookService.create(this.form.value);

    request.subscribe(() => {
      this.closeModal();
      this.form.reset();
      this.onSave.emit();
    });
  }
  closeModal() {
    this.isModalOpen = false;
    this.activeTab = 'basic'; // Reset to first tab
    this.isModalOpenChange.emit(this.isModalOpen);
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  isSupplierSelected(supplierId: string): boolean {
    const selectedSupplierIds = this.form.get('supplierIds')?.value || [];
    return selectedSupplierIds.includes(supplierId);
  }

  onSupplierChange(supplierId: string, isSelected: boolean) {
    const selectedSupplierIds = this.form.get('supplierIds')?.value || [];
    
    if (isSelected) {
      if (!selectedSupplierIds.includes(supplierId)) {
        selectedSupplierIds.push(supplierId);
      }
    } else {
      const index = selectedSupplierIds.indexOf(supplierId);
      if (index > -1) {
        selectedSupplierIds.splice(index, 1);
      }
    }
    
    this.form.patchValue({ supplierIds: selectedSupplierIds });
  }
}
