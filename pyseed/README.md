# Python Supplier Seeding Scripts

Thư mục này chứa các script Python để seed dữ liệu suppliers vào ABP Angular database.

## Yêu cầu

- Python 3.7+
- MySQL connector for Python

## Cài đặt

```bash
pip install -r requirements.txt
```

## Các file chính

### 1. `seed-suppliers-updated.py` (Recommended)
Script chính để seed 20 suppliers với thông tin Việt Nam vào database.

**Cách sử dụng:**
```bash
# Seed bình thường (skip nếu đã có data)
python seed-suppliers-updated.py

# Force seed (thêm mới ngay cả khi có data)
python seed-suppliers-updated.py --force

# Clear data cũ và seed mới
python seed-suppliers-updated.py --clear
```

### 2. `verify-suppliers.py`
Script để kiểm tra và hiển thị tất cả suppliers trong database.

```bash
python verify-suppliers.py
```

### 3. `check-db-structure.py`
Script để kiểm tra cấu trúc bảng AppSuppliers trong database.

```bash
python check-db-structure.py
```

### 4. `run-seed-suppliers.bat`
Batch file Windows để cài đặt dependencies và chạy seeding một cách tự động.

```cmd
run-seed-suppliers.bat
```

## Cấu hình Database

Script sử dụng cấu hình database mặc định:
- Host: localhost
- Port: 3306
- Database: angular
- User: root
- Password: root

Để thay đổi cấu hình, chỉnh sửa biến `DB_CONFIG` trong các file Python.

## Dữ liệu Suppliers

Script sẽ thêm 20 suppliers với thông tin các công ty xuất bản Việt Nam bao gồm:
- Công ty TNHH Sách Giáo Dục Việt Nam
- Nhà xuất bản Trẻ
- Công ty Cổ phần Phát hành Sách TP.HCM
- Nhà xuất bản Kim Đồng
- Và 16 suppliers khác...

Mỗi supplier có đầy đủ thông tin:
- Tên công ty
- Email
- Số điện thoại
- Địa chỉ
- Website
- Trạng thái hoạt động

## Lưu ý

- Script sẽ tự động kiểm tra và tránh duplicate data
- Sử dụng `--force` để thêm suppliers mới ngay cả khi đã có data
- Sử dụng `--clear` để xóa toàn bộ data cũ trước khi seed
- Tất cả operations đều được log chi tiết để theo dõi quá trình

## Book Suppliers Management Scripts

### 1. `verify-book-suppliers.py`
Script kiểm tra và xác minh dữ liệu BookSuppliers hiện tại.

**Chức năng:**
- Kiểm tra cấu trúc database và các bảng cần thiết
- Xác minh cột Suppliers đã được thêm vào bảng AppBooks
- Phân tích dữ liệu hiện tại trong BookSuppliers
- Preview những thay đổi sẽ được thực hiện

**Cách chạy:**
```bash
python verify-book-suppliers.py
# hoặc
run-verify-book-suppliers.bat
```

### 2. `update-book-suppliers.py`
Script cập nhật trường Suppliers trong bảng Books từ dữ liệu BookSuppliers.

**Chức năng:**
- Đọc dữ liệu từ bảng AppBookSuppliers
- Gom nhóm supplier IDs theo BookId
- Cập nhật trường Suppliers trong bảng AppBooks với format comma-separated
- Xác minh và báo cáo kết quả

**Cách chạy:**
```bash
python update-book-suppliers.py
# hoặc
run-update-book-suppliers.bat
```

**Lưu ý quan trọng:**
- Chạy `verify-book-suppliers.py` trước để kiểm tra dữ liệu
- Đảm bảo đã chạy migration để thêm cột Suppliers vào bảng AppBooks
- Script sẽ tự động backup và rollback nếu có lỗi
- Tất cả thay đổi được log chi tiết

### Quy trình khuyến nghị:
1. Chạy migration để thêm cột Suppliers
2. Chạy `verify-book-suppliers.py` để kiểm tra
3. Chạy `update-book-suppliers.py` để cập nhật dữ liệu
4. Kiểm tra kết quả trong database
