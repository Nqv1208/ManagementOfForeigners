# RULE.md

## 1. Naming Conventions (Quy tắc đặt tên)
- **Tên Bảng (Entities)**: Phải khớp 100% với tên trên file ERD (ví dụ: `QuyenHan`, `VaiTro`, `TaiKhoan`, `LichSuCapNhatThongTinCaNhan`, `CanBo`, `NguoiNuocNgoai`, `ChuCoSoLuuTru`, `HoSoKhaiBaoTamTru`, `CoSoLuuTru`, `LichSuCuTru`, `CanhBaoViPham`, `BaoCaoViPham`).
- **Tên Cột (Properties)**: Phải khớp 100% với định nghĩa trong ERD. Viết hoa chữ cái đầu (PascalCase) cho tất cả các public properties.
- **Biến cục bộ / Tham số**: Sử dụng camelCase.
- **Interface**: Tiền tố `I` (ví dụ: `IRepository`).

## 2. Kiểu dữ liệu & Khóa chính (Primary Keys)
- Các bảng sử dụng khóa chính dạng chuỗi phải định dạng là `CHAR(9)`.
- Giá trị khóa chính dạng chuỗi được tạo tự động thông qua `IdGenerator` helper với dạng `Prefix + 6 chữ số` (VD: `TK000001`, `NN000001`).
- Tuyệt đối không dùng `DatabaseGeneratedOption.Identity` cho các cột `CHAR(9)`.
- Các bảng sử dụng khóa chính dạng số nguyên (INT) như `VaiTro`, `QuyenHan` có thể tự định nghĩa hoặc dùng Identity nếu cần, nhưng phải nhất quán với thiết kế. Đối với `VaiTro` thì `ValueGeneratedNever` nếu seed data cứng. `QuyenHan` có thể tự tăng hoặc cung cấp thủ công. `CanhBaoViPham` dùng `INT` (Identity).

## 3. Entity Framework Core (Database Design)
- Tất cả mapping cấu trúc, ràng buộc khóa ngoại, độ dài ký tự (`HasMaxLength`, `IsFixedLength`) phải được định nghĩa trong `OnModelCreating` của `ApplicationDbContext.cs` thông qua Fluent API. Không sử dụng Data Annotations trong Entity class.
- Xác định rõ hành vi khi xóa (Delete Behavior): `Restrict`, `Cascade`, `SetNull` theo đúng logic nghiệp vụ để tránh lỗi Cascade cycles.

## 4. Xác thực và Phân quyền (Authentication & Authorization)
- Chỉ sử dụng **Cookie Authentication** và **Session**, không sử dụng JWT Token.
- Phân quyền tại các Controller/Action phải dùng bộ lọc tùy chỉnh `[AuthorizeRole("RoleName")]`.
- Các Role được định nghĩa sẵn trong hệ thống: `Foreigner`, `Owner`, `WardPolice`, `Officer`.

## 5. Security & Audit (Bảo mật)
- Thông tin cá nhân nhạy cảm của Người nước ngoài bị chỉnh sửa phải được ghi nhận lịch sử vào bảng `LichSuCapNhatThongTinCaNhan`.
- Không bao giờ trả về nguyên bản (plain text) hoặc chuỗi Hash của mật khẩu cho bất kỳ endpoint nào (View hoặc API json).
- Phải Validate chặt chẽ dữ liệu đầu vào thông qua ViewModels và ModelState.

## 6. Project Architecture
- Tuân thủ mô hình **ASP.NET Core MVC** truyền thống (Model - View - Controller). 
- Toàn bộ giao diện được render qua **Razor Views** (server-side).
- Các request xử lý dữ liệu phức tạp trên giao diện có thể gọi Ajax, nhưng ưu tiên Controller xử lý logic chính và render Partial View hoặc Redirect.

## 7. Migration & Seed Data
- Bất kỳ khi nào thay đổi Entities, phải tạo migration mới.
- Quá trình SeedData.cs cần được duy trì cẩn thận, đảm bảo dữ liệu chạy đúng cho các Role Demo ngay lần khởi chạy đầu tiên để kiểm thử.
