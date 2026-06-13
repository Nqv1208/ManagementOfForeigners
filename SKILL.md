# SKILL.md

## Skill: Core Foreign Resident Management

Skill này hướng dẫn AI Agent triển khai các chức năng cốt lõi của hệ thống quản lý người nước ngoài theo đúng kiến trúc ASP.NET Core MVC (Razor Views). Toàn bộ Database Entities phải tuân thủ nghiêm ngặt 100% ERD.

## 1. Khi nào dùng skill này
Dùng khi task liên quan đến:
- Phát triển tính năng cho các Role (Guest, Foreigner, AccommodationOwner, ImmigrationOfficer, WardPolice).
- Tạo hoặc cập nhật các Controller, View, ViewModels.
- Thao tác, cập nhật thông tin qua DbContext (Entity Framework Core).

Không dùng skill này để tự ý triển khai:
- Các chức năng ngoài phạm vi (như Visa payment, thẻ tạm trú v.v.)
- Đổi công nghệ (ví dụ sang React/Vue hoặc dùng JWT auth thay cho Cookie).

## 2. Actors & Quyền Hạn
- **Guest**: Xem tin tức, đăng ký tài khoản.
- **Foreigner (Người Nước Ngoài)**: Đăng nhập, khai báo tạm trú, cập nhật thông tin cá nhân.
- **AccommodationOwner (Chủ cơ sở lưu trú)**: Khai báo lưu trú, xem danh sách, đổi trạng thái cư trú của khách.
- **WardPolice (Công an Phường/Xã)**: Duyệt hồ sơ khai báo tạm trú trên địa bàn, cảnh báo vi phạm, báo cáo cấp trên.
- **ImmigrationOfficer (Cán bộ XNC)**: Tra cứu hệ thống, gửi cảnh báo, xem thống kê.

## 3. Cấu trúc Database (Chuẩn 100% ERD)
Hệ thống sử dụng chính xác các bảng sau:
1. `VaiTro`
2. `QuyenHan`
3. `TaiKhoan`
4. `LichSuCapNhatThongTinCaNhan`
5. `CanBo`
6. `NguoiNuocNgoai`
7. `ChuCoSoLuuTru`
8. `CoSoLuuTru`
9. `HoSoKhaiBaoTamTru`
10. `LichSuCuTru`
11. `CanhBaoViPham`
12. `BaoCaoViPham`

(Sự khác biệt với phiên bản cũ là gộp `LichSuLuuTru`/`LichSuCuTru` thành 1, và dùng đúng tên `QuyenHan`, `LichSuCapNhatThongTinCaNhan`)

## 4. Business Rules & Validations
- **ID Generation**: Mã định danh (MaTaiKhoan, MaNguoiNuocNgoai, v.v...) được gen động với prefix + số (VD: TK000001, NN000001) với độ dài CHAR(9).
- **Date Validation**: Các trường ngày `NgayKetThuc` luôn phải >= `NgayBatDau`. `NgaySinh` phải hợp lệ.
- **Status Constants**: Nên tạo các Struct/Enum mô tả đúng trạng thái: `Chờ duyệt`, `Đã duyệt`, `Từ chối` đối với Hồ sơ, và `Đang ở`, `Đã rời` đối với Lưu trú.
- **Cascade Deletes**: Hạn chế sử dụng Cascade Delete (No Action/Restrict) giữa các bảng nghiệp vụ nhạy cảm như (NguoiNuocNgoai - TaiKhoan - CanhBao) để tránh xóa nhầm dữ liệu diện rộng.

## 5. Coding & Setup Guidelines
- Phân quyền sử dụng Filter: `[AuthorizeRole("Foreigner")]`.
- Sử dụng EF Core Fluent API trong `ApplicationDbContext.cs` để cấu hình thay vì Data Annotations.
- Form trong Razor View sử dụng `asp-for` và jQuery Validation Unobtrusive cho thao tác validate phía client.
