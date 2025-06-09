import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { BookService, BookDto } from '../proxy/books';

@Component({
  standalone: false,
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss'],
  providers: [ListService],
})
export class BookComponent implements OnInit {
  book = { items: [], totalCount: 0 } as PagedResultDto<BookDto>;

  selectedBook = {} as BookDto;

  isModalOpen = false;

  filter = '';

  constructor(
    public readonly list: ListService,
    private bookService: BookService,
    private confirmation: ConfirmationService
  ) {}
  ngOnInit() {
    const bookStreamCreator = (query) => {
      return this.bookService.getList({
        ...query,
        filter: this.filter
      });
    };

    this.list.hookToQuery(bookStreamCreator).subscribe((response) => {
      this.book = response;
    });
  }
  createBook() {
    this.selectedBook = {} as BookDto;
    this.isModalOpen = true;
  }

  editBook(id: string) {
    this.bookService.get(id).subscribe((book) => {
      this.selectedBook = book;
      this.isModalOpen = true;
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.bookService.delete(id).subscribe(() => this.list.get());
      }
    });
  }
  onModalSave() {
    this.list.get();
  }

  onFilterChange() {
    this.list.get();
  }

  clearFilter() {
    this.filter = '';
    this.list.get();
  }
}
