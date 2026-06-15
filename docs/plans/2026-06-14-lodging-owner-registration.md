# Lodging Owner Registration Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implement registration select page, a dedicated registration form, database schema updates (nullable fields like LoaiHinh, MaSoKinhDoanh, SoPhong, SucChuaToiDa, CreatedAt, UpdatedAt), and admin approval mechanisms for lodging owners.

**Architecture:** We will create a select page at `Views/TaiKhoan/DangKy.cshtml` to choose the account type. Foreigner registration will be moved to `DangKyNguoiNuocNgoai.cshtml`. Lodging owner registration will be placed in `DangKyCoSoLuuTru.cshtml` powered by `DangKyCoSoLuuTruViewModel`. The registered accounts/facilities will start in a "Chờ duyệt" status.

**Tech Stack:** ASP.NET Core 10.0 MVC, Entity Framework Core, SQL Server, C#, Razor.

---

### Task 1: Update CoSoLuuTru Entity and ApplicationDbContext
Add new properties to `CoSoLuuTru` and map them in the DbContext.

**Files:**
- Modify: `Models/Entities/CoSoLuuTru.cs`
- Modify: `Data/ApplicationDbContext.cs`

**Step 1: Modify CoSoLuuTru.cs**
Add the following properties:
```csharp
    public string? LoaiHinh { get; set; }
    public string? MaSoKinhDoanh { get; set; }
    public int? SoPhong { get; set; }
    public int? SucChuaToiDa { get; set; }
    public string? GhiChu { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
```
Also add `public const string ChoDuyet = "Chờ duyệt";` to `TrangThaiCoSo`.

**Step 2: Modify ApplicationDbContext.cs**
Inside `OnModelCreating` for `CoSoLuuTru`, map the new fields:
```csharp
            e.Property(x => x.LoaiHinh).HasMaxLength(50);
            e.Property(x => x.MaSoKinhDoanh).HasMaxLength(50);
            e.Property(x => x.SoPhong).HasColumnType("int");
            e.Property(x => x.SucChuaToiDa).HasColumnType("int");
            e.Property(x => x.GhiChu).HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.Property(x => x.UpdatedAt).HasColumnType("datetime");
```

**Step 3: Create EF Migration and Update Database**
Run migrations to update the database schema:
Run: `dotnet ef migrations add AddLodgingOwnerRegistration`
Run: `dotnet ef database update`

---

### Task 2: Create ViewModels
Create ViewModels for the lodging owner and foreigner registration flows.

**Files:**
- Create: `Models/ViewModels/DangKyCoSoLuuTruViewModel.cs`
- Create: `Models/ViewModels/DangKyNguoiNuocNgoaiViewModel.cs`

**Step 1: Create DangKyCoSoLuuTruViewModel.cs**
Declare all fields for account, representative, lodging facility, select list option, and agreement:
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementOfForeigners.Models.ViewModels;

public class DangKyCoSoLuuTruViewModel
{
    // Tài khoản
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    // Chủ cơ sở
    [Required(ErrorMessage = "Họ và tên chủ cơ sở không được để trống")]
    public string HoTenChuCoSo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số giấy tờ không được để trống")]
    public string SoGiayTo { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChiLienHe { get; set; }

    public string? ChucVu { get; set; }

    // Cơ sở lưu trú
    [Required(ErrorMessage = "Tên cơ sở lưu trú không được để trống")]
    public string TenCoSoLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại hình cơ sở")]
    public string LoaiHinhCoSo { get; set; } = string.Empty;

    public string? MaSoKinhDoanh { get; set; }

    [Required(ErrorMessage = "Số điện thoại cơ sở không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại cơ sở không hợp lệ")]
    public string SoDienThoaiCoSo { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email cơ sở không hợp lệ")]
    public string? EmailCoSo { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
    public int? PhuongXaId { get; set; }

    [Required(ErrorMessage = "Địa chỉ cụ thể không được để trống")]
    public string DiaChiCuThe { get; set; } = string.Empty;

    public int? SoPhong { get; set; }

    public int? SucChuaToiDa { get; set; }

    public string? GhiChu { get; set; }

    [Required(ErrorMessage = "Vui lòng xác nhận cam kết thông tin")]
    public bool CamKetThongTin { get; set; }
}
```

**Step 2: Create DangKyNguoiNuocNgoaiViewModel.cs**
Re-use the current properties of `DangKyViewModel` but filter down to the foreigner profile registration.

---

### Task 3: Update Controller Actions
Add Controller actions in `TaiKhoanController.cs` to handle both registration flows, and redirect to a success page.

**Files:**
- Modify: `Controllers/TaiKhoanController.cs`

**Step 1: Modify TaiKhoanController.cs**
- Implement GET `/taikhoan/dangky` (returns selection view).
- Implement GET `/taikhoan/dangky-nguoinuocngoai` (returns foreigner form).
- Implement POST `/taikhoan/dangky-nguoinuocngoai` (saves account + foreigner).
- Implement GET `/taikhoan/dangky-cosoluutru` (populates ward lists, returns lodging owner form).
- Implement POST `/taikhoan/dangky-cosoluutru` (saves account with status "Chờ duyệt", owner with status "Chờ duyệt", lodging facility with status "Chờ duyệt"). Do not auto-login lodging owners on success.
- Implement GET `/taikhoan/dangky-thanhcong` (returns success message page).

---

### Task 4: Create Views
Implement the views for select page, lodging owner form, foreigner form, and success page.

**Files:**
- Modify: `Views/TaiKhoan/DangKy.cshtml` (Select page)
- Create: `Views/TaiKhoan/DangKyNguoiNuocNgoai.cshtml` (Foreigner form)
- Create: `Views/TaiKhoan/DangKyCoSoLuuTru.cshtml` (Lodging owner form)
- Create: `Views/TaiKhoan/DangKyThanhCong.cshtml` (Success page)
- Create: `wwwroot/css/account-register.css`

---

### Task 5: Handle Authentication and Login Blocks for Pending Accounts
Ensure that pending accounts ("Chờ duyệt") are blocked from logging in or redirected to a warning page.

**Files:**
- Modify: `Controllers/TaiKhoanController.cs` (in login POST action check if status is "Chờ duyệt")

**Step 1: Modify login post action**
In `TaiKhoanController.DangNhap(...)`:
If user status is `TrangThaiTaiKhoan.ChoDuyet`, return validation error: "Tài khoản của bạn đang chờ quản trị viên duyệt."

---

### Task 6: Admin Actions for Approving Facilities
Enable Admin to view and approve pending lodging owner accounts in the admin view.

**Files:**
- Modify: `Controllers/QuanTriController.cs`
- Modify: `Views/QuanTri/TaiKhoan.cshtml`
- Modify: `Views/QuanTri/CoSoLuuTru.cshtml`

---

### Task 7: Build and Verify
Verify build and tests.
Run: `dotnet build`
Run: `dotnet run --project Tests/SchemaVerification/SchemaVerification.csproj`
