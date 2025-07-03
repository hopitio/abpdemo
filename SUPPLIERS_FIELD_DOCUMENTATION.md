# Bổ sung trường Suppliers vào bảng Books

## Mô tả
Đã thêm trường `Suppliers` vào bảng `Books` để lưu trữ danh sách ID supplier dưới dạng chuỗi (comma-separated string), bổ sung cho bảng many-to-many `BookSuppliers` hiện có.

## Các thay đổi đã thực hiện

### 1. Domain Layer
- **Book.cs**: Thêm property `Suppliers` (string) để lưu trữ các supplier ID cách nhau bằng dấu phẩy
- **SupplierHelper.cs**: Tạo helper class chứa các phương thức tiện ích:
  - `ConvertStringToSupplierIds()`: Chuyển đổi từ string sang List<Guid>
  - `ConvertSupplierIdsToString()`: Chuyển đổi từ List<Guid> sang string
  - `AddSupplierToString()`: Thêm supplier ID vào chuỗi
  - `RemoveSupplierFromString()`: Xóa supplier ID khỏi chuỗi

### 2. EntityFrameworkCore Layer
- **AbpAngularDbContext.cs**: Cập nhật cấu hình Entity Framework cho trường `Suppliers` với độ dài tối đa 512 ký tự
- **Migration**: Tạo migration "AddSuppliersColumnToBooks" để thêm cột mới vào database

### 3. Application Layer
- **BookDto.cs**: Thêm property `SuppliersString` để hiển thị danh sách supplier ID dưới dạng chuỗi
- **CreateUpdateBookDto.cs**: Thêm property `Suppliers` để nhận input từ client
- **BookAppService.cs**: Cập nhật logic xử lý:
  - Chuyển đổi giữa string và List<Guid> khi tạo/cập nhật book
  - Đồng bộ dữ liệu giữa trường `Suppliers` và bảng `BookSuppliers`
- **AutoMapper Profile**: Cập nhật mapping giữa Entity và DTO

## Cách sử dụng

### Từ API/Client
```csharp
// Tạo book mới với suppliers
var bookDto = new CreateUpdateBookDto
{
    Name = "Sample Book",
    Type = BookType.Fiction,
    PublishDate = DateTime.Now,
    Price = 29.99f,
    Suppliers = "123e4567-e89b-12d3-a456-426614174000,987fcdeb-51a2-43c1-b123-456789abcdef"
};
```

### Từ Database
```sql
-- Truy vấn books với suppliers
SELECT Id, Name, Price, Suppliers 
FROM AppBooks 
WHERE Suppliers IS NOT NULL AND Suppliers != '';
```

## Lợi ích
1. **Hiệu suất**: Truy vấn nhanh hơn khi chỉ cần danh sách ID supplier
2. **Đơn giản**: Dễ dàng kiểm tra xem book có supplier cụ thể hay không
3. **Tương thích**: Vẫn giữ nguyên bảng many-to-many `BookSuppliers` cho các truy vấn phức tạp
4. **Linh hoạt**: Có thể sử dụng cả hai cách tùy theo nhu cầu

## Lưu ý
- Trường `Suppliers` có độ dài tối đa 512 ký tự
- Dữ liệu được đồng bộ tự động giữa trường `Suppliers` và bảng `BookSuppliers`
- Khi cập nhật book, hệ thống sẽ tự động cập nhật cả hai nơi lưu trữ
