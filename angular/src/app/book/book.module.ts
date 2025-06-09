import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { BookRoutingModule } from './book-routing.module';
import { BookComponent } from './book.component';
import { BookModalComponent } from './book-modal/book-modal.component';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    BookComponent,
    BookModalComponent
  ],
  imports: [
    BookRoutingModule,
    SharedModule,
    NgbDatepickerModule,
  ]
})
export class BookModule { }
