# Vietnamese System Synchronization Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Synchronize the entire project's controllers, viewmodels, views, routes, folder names, and file names from English to Vietnamese, ensuring that routing matches the new Vietnamese names and all navigation remains fully functional.

**Architecture:** We will rename controller files and classes (e.g., `AccountController` -> `TaiKhoanController`), update view folders, view filenames, namespaces, redirections, and all view references (`asp-controller`/`asp-action`). Since default MVC routing maps routing conventions directly to controller/action names, all routes will automatically align to Vietnamese.

**Tech Stack:** ASP.NET Core 10.0 MVC, EF Core, C#, Razor (.cshtml).

---

### Task 1: Rename Controller Files and Classes
Rename all controllers to Vietnamese and update their class names and namespaces.

**Files:**
- Create: `Controllers/TrangChuController.cs`
- Create: `Controllers/TaiKhoanController.cs`
- Create: `Controllers/CoSoLuuTruController.cs`
- Create: `Controllers/QuanTriController.cs`
- Create: `Controllers/NguoiNuocNgoaiController.cs`
- Create: `Controllers/CanBoController.cs`
- Create: `Controllers/CongAnController.cs`
- Delete: `Controllers/HomeController.cs`
- Delete: `Controllers/AccountController.cs`
- Delete: `Controllers/AccommodationController.cs`
- Delete: `Controllers/AdminController.cs`
- Delete: `Controllers/ForeignerController.cs`
- Delete: `Controllers/OfficerController.cs`
- Delete: `Controllers/PoliceController.cs`

**Step 1: Create new controllers with translated code**
Write the translated code for each controller, updating class names, constructors, and actions:
- `HomeController` -> `TrangChuController` (Actions: `Index`, `Privacy` -> `ChinhSachBaoMat`, `Error` -> `Loi`)
- `AccountController` -> `TaiKhoanController` (Actions: `Login` -> `DangNhap`, `Register` -> `DangKy`, `Logout` -> `DangXuat`, `AccessDenied` -> `TuChoiTruyCap`)
- `AccommodationController` -> `CoSoLuuTruController` (Action: `Dashboard` -> `TongQuan`, others stay as is)
- `AdminController` -> `QuanTriController` (Action: `Dashboard` -> `TongQuan`, others stay as is)
- `ForeignerController` -> `NguoiNuocNgoaiController` (Action: `Dashboard` -> `TongQuan`, others stay as is)
- `OfficerController` -> `CanBoController` (Action: `Dashboard` -> `TongQuan`, others stay as is)
- `PoliceController` -> `CongAnController` (Action: `Dashboard` -> `TongQuan`, others stay as is)

**Step 2: Delete old controller files**
Ensure the old English controllers are deleted.

---

### Task 2: Rename and Update ViewModel Folders, Files and Namespaces
Translate and relocate the ViewModels folder structure and update the C# class declarations.

**Files:**
- Relocate folder: `Models/ViewModels/Accommodation` -> `Models/ViewModels/CoSoLuuTru`
- Relocate folder: `Models/ViewModels/Admin` -> `Models/ViewModels/QuanTri`
- Relocate folder: `Models/ViewModels/Foreigner` -> `Models/ViewModels/NguoiNuocNgoai`
- Relocate folder: `Models/ViewModels/Officer` -> `Models/ViewModels/CanBo`
- Relocate folder: `Models/ViewModels/Police` -> `Models/ViewModels/CongAn`
- Create: `Models/ViewModels/DangKyViewModel.cs`
- Create: `Models/ViewModels/DangNhapViewModel.cs`
- Delete: `Models/ViewModels/RegisterViewModel.cs`
- Delete: `Models/ViewModels/LoginViewModel.cs`
- Rename ViewModels inside subfolders:
  - `DashboardViewModel.cs` -> `TongQuanViewModel.cs` (in all subfolders)
  - `AuditEntryViewModel.cs` -> `NhatKyHoatDongViewModel.cs`

**Step 1: Relocate and translate viewmodels**
Recreate the ViewModel files in the new paths with matching namespaces and class names, then delete the old files.

---

### Task 3: Rename and Relocate Views Folders and Files
Translate the View folder structure and filenames to match the new controller and action names.

**Files:**
- Relocate views folders:
  - `Views/Home` -> `Views/TrangChu`
  - `Views/Account` -> `Views/TaiKhoan`
  - `Views/Accommodation` -> `Views/CoSoLuuTru`
  - `Views/Admin` -> `Views/QuanTri`
  - `Views/Foreigner` -> `Views/NguoiNuocNgoai`
  - `Views/Officer` -> `Views/CanBo`
  - `Views/Police` -> `Views/CongAn`
- Rename view files:
  - `Views/TrangChu/Privacy.cshtml` -> `Views/TrangChu/ChinhSachBaoMat.cshtml`
  - `Views/TaiKhoan/Login.cshtml` -> `Views/TaiKhoan/DangNhap.cshtml`
  - `Views/TaiKhoan/Register.cshtml` -> `Views/TaiKhoan/DangKy.cshtml`
  - `Views/TaiKhoan/AccessDenied.cshtml` -> `Views/TaiKhoan/TuChoiTruyCap.cshtml`
  - `Views/CoSoLuuTru/Dashboard.cshtml` -> `Views/CoSoLuuTru/TongQuan.cshtml`
  - `Views/QuanTri/Dashboard.cshtml` -> `Views/QuanTri/TongQuan.cshtml`
  - `Views/NguoiNuocNgoai/Dashboard.cshtml` -> `Views/NguoiNuocNgoai/TongQuan.cshtml`
  - `Views/CanBo/Dashboard.cshtml` -> `Views/CanBo/TongQuan.cshtml`
  - `Views/CongAn/Dashboard.cshtml` -> `Views/CongAn/TongQuan.cshtml`

---

### Task 4: Update All Namespace References, Actions, Links and Routing Config
Update all code files to align with the new controller, action, view, and viewmodel names.

**Files:**
- Modify: `Program.cs`
- Modify: `Views/_ViewImports.cshtml`
- Modify: `Filters/AuthorizeRoleAttribute.cs`
- Modify: All CSHTML views in `Views/` (updating `@model`, `asp-controller`, `asp-action`, and dashboard redirects)
- Modify: All Controllers in `Controllers/` (updating namespaces, `RedirectToAction` calls, and models)

**Step 1: Update Program.cs**
Update default route settings from `controller=Home, action=Index` to `controller=TrangChu, action=Index`.

**Step 2: Update ViewImports and AuthorizeRoleAttribute**
Update imports and authentication redirects (`"Login", "Account"` -> `"DangNhap", "TaiKhoan"`, `"AccessDenied", "Account"` -> `"TuChoiTruyCap", "TaiKhoan"`).

**Step 3: Update links and references in Views**
Run global replacements or file edits in all views to update layout references, viewmodels, and menu links.

---

### Task 5: Compilation and Verification Testing
Verify that everything compiles cleanly and all database-related and UI checks pass.

**Step 1: Run compilation**
Run: `dotnet build`
Expected: Build succeeded with 0 errors.

**Step 2: Run verification program**
Run: `dotnet run --project Tests/SchemaVerification/SchemaVerification.csproj`
Expected: All validation checks PASS.
