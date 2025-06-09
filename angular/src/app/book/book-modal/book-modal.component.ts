import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { BookService, BookDto, bookTypeOptions } from '../../proxy/books';

@Component({
  standalone: false,
  selector: 'app-book-modal',
  templateUrl: './book-modal.component.html',
  styleUrls: ['./book-modal.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class BookModalComponent implements OnChanges {
  @Input() isModalOpen = false;
  @Input() selectedBook = {} as BookDto;
  @Output() isModalOpenChange = new EventEmitter<boolean>();
  @Output() onSave = new EventEmitter<void>();

  form: FormGroup;
  bookTypes = bookTypeOptions;
  activeTab = 'basic'; // Default tab

  constructor(
    private bookService: BookService,
    private fb: FormBuilder
  ) {
    this.buildForm();
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
}
