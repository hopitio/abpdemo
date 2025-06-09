# Book Modal Component

## Mô tả
Component `BookModalComponent` đã được tách ra từ `BookComponent` để tái sử dụng và dễ dàng bảo trì.

## Cách sử dụng

### 1. Import trong module
```typescript
import { BookModalComponent } from './book-modal/book-modal.component';

@NgModule({
  declarations: [
    BookModalComponent
  ],
  // ...
})
```

### 2. Sử dụng trong template
```html
<app-book-modal
  [(isModalOpen)]="isModalOpen"
  [selectedBook]="selectedBook"
  (onSave)="onModalSave()">
</app-book-modal>
```

## Input Properties
- `isModalOpen: boolean` - Trạng thái hiển thị modal
- `selectedBook: BookDto` - Dữ liệu book được chọn để edit (trống khi tạo mới)

## Output Events
- `isModalOpenChange: EventEmitter<boolean>` - Sự kiện thay đổi trạng thái modal
- `onSave: EventEmitter<void>` - Sự kiện khi save thành công

## Tính năng
- Tự động build form dựa trên selectedBook
- Validation form
- Gọi API create/update tự động
- Đóng modal và reset form sau khi save thành công
- Emit event để parent component refresh data

## Cấu trúc Files
```
book-modal/
├── book-modal.component.ts      # Logic component
├── book-modal.component.html    # Template
├── book-modal.component.scss    # Styles
└── book-modal.component.spec.ts # Unit tests
```
