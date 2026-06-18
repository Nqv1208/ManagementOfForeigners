# Kế hoạch cải thiện giao diện Cổng Thông tin Công an Phường/Xã

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Cải thiện toàn diện giao diện và trải nghiệm người dùng (UI/UX) của Cổng Thông tin Công an Phường/Xã (Police Portal) để đạt tiêu chuẩn chuyên nghiệp, hiện đại, tối ưu hóa phân cấp thị giác và các hành động nghiệp vụ.

**Architecture:** Áp dụng hệ thống thiết kế (Design System) mới: thay thế các header card màu xanh lá đậm thô bằng thiết kế card trắng có viền trái (border-left) xanh lá; sử dụng segmented control cho các tab bộ lọc; tối ưu hóa mật độ hiển thị bảng biểu và form mẫu.

**Tech Stack:** ASP.NET Core MVC (Razor), Vanilla CSS, Bootstrap Icons, jQuery và Bootstrap 5.

---

### Task 1: Xây dựng CSS Design System và Cấu trúc Layout

**Files:**
- Modify: `wwwroot/css/police-layout.css`
- Modify: `Views/Shared/_PoliceLayout.cshtml`

**Step 1: Cập nhật CSS Layout và tạo các Component dùng chung**
Thêm các class mới vào `wwwroot/css/police-layout.css` để định hình thiết kế hiện đại:
- Nền trang `.police-content` với màu sáng `#f6f8f5` kết hợp radial-gradient xanh lá rất nhẹ ở góc phải.
- Hero card `.police-hero` và card header `.police-page-header`: nền trắng, viền trái `4px solid var(--primary-dark)`, bo góc `16px`, shadow rất nhẹ.
- Segmented control `.filter-tabs`: khối nền xám nhạt, các tab con là `.filter-tab` bo tròn, chuyển đổi trạng thái mượt mà.
- Thiết kế table `.police-table-themed`: header table nền xanh nhạt `#eaf7e6`, text màu xanh lá đậm, hover hàng nhẹ nhàng, các button hành động dạng outline hoặc có độ tương phản thấp hơn để không làm rối dữ liệu.

**Step 2: Cập nhật file _PoliceLayout.cshtml**
- Cập nhật class `.police-content` trong `_PoliceLayout.cshtml` để đảm bảo hệ thống màu nền mới được áp dụng đồng bộ.
- Xóa bỏ hoặc làm sạch mọi watermark logo nền ở content chính nếu có, thay thế bằng background sạch đẹp.

**Step 3: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 4: Commit**
```bash
git add wwwroot/css/police-layout.css Views/Shared/_PoliceLayout.cshtml
git commit -m "style(police): implement modern design system and clean layout background"
```

---

### Task 2: Cải thiện Trang Tổng quan (Dashboard)

**Files:**
- Modify: `Views/CongAn/TongQuan.cshtml`

**Step 1: Tinh chỉnh Hero Banner địa bàn**
- Giảm chiều cao hero banner xuống khoảng 25-30%.
- Thay thế class `.gov-hero-card` (nền xanh đậm) bằng class `.police-hero` mới (nền trắng, border-left xanh lá đậm).
- Chuyển đổi màu chữ tiêu đề thành màu tối `#1f2937` để hiển thị rõ ràng trên nền trắng.
- Tinh chỉnh các button hành động chính thành các button outline hoặc có kích thước nhỏ gọn hơn.

**Step 2: Chuẩn hóa 4 Stat Cards phân tích số liệu**
- Cập nhật cấu trúc của 4 thẻ phân tích:
  - Chỉ riêng thẻ **Chờ duyệt tạm trú** là sử dụng class `.is-warning` (nền vàng/amber nhạt).
  - 3 thẻ còn lại (Đã duyệt, NNN trong địa bàn, Cảnh báo đã gửi) sử dụng nền trắng, border mỏng và shadow nhẹ để tạo sự cân bằng và phân cấp rõ ràng.

**Step 3: Cập nhật bảng "Chờ duyệt" và danh sách "Cảnh báo"**
- Áp dụng class `.police-table-themed` cho bảng danh sách chờ duyệt.
- Tinh chỉnh khoảng cách (padding/margin) và cỡ chữ của phần danh sách cảnh báo bên phải.

**Step 4: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 5: Commit**
```bash
git add Views/CongAn/TongQuan.cshtml
git commit -m "feat(police): update dashboard layout with operational hero and refined stat cards"
```

---

### Task 3: Refactor Trang Danh sách & Phê duyệt Khai báo

**Files:**
- Modify: `Controllers/CongAnController.cs`
- Modify: `Views/CongAn/DanhSachKhaiBao.cshtml`

**Step 1: Thêm số liệu thống kê số lượng hồ sơ tạm trú vào Controller**
Trong action `DanhSachKhaiBao` của `CongAnController.cs`, tính toán số lượng hồ sơ theo từng trạng thái và truyền qua ViewBag để hiển thị trên tab bộ lọc:
```csharp
var baseQuery = WardDeclarations(canBo.MaPhuongXa);
ViewBag.CountPending = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
ViewBag.CountApproved = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.DaDuyet);
ViewBag.CountRejected = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.TuChoi);
```

**Step 2: Refactor layout và bộ lọc Segmented Control**
- Thay thế header xanh lá đậm bằng card trắng `.police-page-header` viền trái xanh.
- Chuyển bộ lọc trạng thái thành dạng Segmented Control nhẹ nhàng:
  - Hiển thị số lượng hồ sơ tương ứng: `Chờ duyệt (@ViewBag.CountPending)`, `Đã duyệt (@ViewBag.CountApproved)`, `Từ chối (@ViewBag.CountRejected)`.
- Cân chỉnh lại vị trí ô tìm kiếm (Search input) để nằm cân đối ở phía bên phải bộ lọc.

**Step 3: Polish bảng danh sách hồ sơ khai báo**
- Áp dụng class `.police-table-themed` cho bảng dữ liệu.
- Thay đổi nút hành động "Chi tiết" thành dạng outline button `btn-outline-success` nhỏ gọn để giảm sắc xanh đặc lặp lại liên tục.
- Sử dụng các badge trạng thái nhỏ và tinh tế.

**Step 4: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 5: Commit**
```bash
git add Controllers/CongAnController.cs Views/CongAn/DanhSachKhaiBao.cshtml
git commit -m "feat(police): redesign declaration list with segmented filter tabs and polished table"
```

---

### Task 4: Refactor Trang Tra cứu Địa bàn

**Files:**
- Modify: `Views/CongAn/TraCuu.cshtml`

**Step 1: Cập nhật Header và Thanh Tìm kiếm**
- Chuyển header card xanh lá thành page header trắng `.police-page-header`.
- Tinh chỉnh chiều cao và kích thước của nút "Tìm kiếm".

**Step 2: Thêm Panel "Bộ lọc nâng cao" dạng Collapsible**
- Bổ sung bộ lọc mở rộng cho phép lọc theo các tiêu chí: Quốc tịch, Loại Visa, Tình trạng cư trú.
- Mặc định ẩn bộ lọc này bên trong nút "Bộ lọc nâng cao" để tránh làm rối giao diện tra cứu ban đầu.

**Step 3: Polish bảng kết quả tra cứu**
- Chuyển đổi các nút hành động đặc màu đỏ ("Báo cáo XNC") và vàng ("Cảnh báo") trên từng dòng dữ liệu thành dạng outline button gọn gàng:
  - Nút "Cảnh báo" dùng class `btn-outline-warning`.
  - Nút "Báo cáo XNC" dùng class `btn-outline-danger`.
- Thêm cột thao tác chính là "Xem hồ sơ" (dạng link/outline nhẹ) để người dùng tập trung đọc thông tin trước khi thực hiện hành động.

**Step 4: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 5: Commit**
```bash
git add Views/CongAn/TraCuu.cshtml
git commit -m "feat(police): refine search view with advanced filters collapse and outline actions"
```

---

### Task 5: Refactor Trang Tạo cảnh báo Vi phạm

**Files:**
- Modify: `Views/CongAn/CanhBaoViPham.cshtml`

**Step 1: Cải thiện bố cục form nhập liệu**
- Sử dụng cấu trúc layout 2 cột rõ rệt:
  - Cột trái (65%): Form điền thông tin vi phạm.
  - Cột phải (35%): Panel thông tin người nước ngoài được chọn + Hướng dẫn chi tiết về các mức độ vi phạm.
- Thay card header xanh đặc bằng card trắng viền trái xanh nhã nhặn.

**Step 2: Refactor ô chọn loại vi phạm thành Preset Dropdown**
- Sử dụng thẻ `select` với các loại vi phạm được định nghĩa trước (presets):
  - `Chưa khai báo tạm trú`
  - `Quá hạn lưu trú`
  - `Không đúng địa chỉ cư trú`
  - `Thông tin hộ chiếu/visa không khớp`
  - `Có dấu hiệu vi phạm quy định cư trú`
  - `Khác`
- Hiển thị ô nhập text tự do (`LoaiViPhamCustom`) chỉ khi người dùng chọn giá trị "Khác".

**Step 3: Polish Modal Xác nhận và Script tương tác**
- Tinh chỉnh thiết kế modal xác nhận trước khi gửi cảnh báo để hiển thị rõ tên người nước ngoài, hành vi và mức độ vi phạm với màu sắc badge tương ứng.
- Đảm bảo font chữ và khoảng cách trong form đều tuân thủ Design System (radius 12px, inputs height 42px).

**Step 4: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 5: Commit**
```bash
git add Views/CongAn/CanhBaoViPham.cshtml
git commit -m "feat(police): update warning form layout and enforce violation type presets"
```

---

### Task 6: Refactor Trang Báo cáo Vi phạm

**Files:**
- Modify: `Views/CongAn/BaoCaoViPham.cshtml`

**Step 1: Đồng bộ thiết kế trang báo cáo vi phạm**
- Chuyển header card thành page header trắng với viền trái đỏ `.police-page-header` (màu đỏ biểu trưng cho vi phạm nghiêm trọng).
- Tinh chỉnh các input form, border, shadow và các button hành động để đồng bộ hoàn toàn với cấu trúc trang tạo cảnh báo.

**Step 2: Chạy build kiểm tra cú pháp**
Run: `dotnet build`
Expected: PASS

**Step 3: Commit**
```bash
git add Views/CongAn/BaoCaoViPham.cshtml
git commit -m "feat(police): unify report violation view styling and layout spacing"
```

---

### Task 7: Build & Validation

**Step 1: Chạy build toàn bộ project**
Run: `dotnet build`
Expected: BUILD SUCCESS với 0 Warnings, 0 Errors.

**Step 2: Thực hiện kiểm tra trực quan các view**
Đảm bảo tất cả các trang của Cổng Công an Phường/Xã hoạt động trơn tru, giao diện thống nhất, mượt mà và trực quan.
