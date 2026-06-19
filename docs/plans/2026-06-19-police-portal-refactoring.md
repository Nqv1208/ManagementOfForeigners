# Police Portal UI/UX Polish Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Polish the Ward Police Portal (CongAn role) in the ManagementOfForeigners project to elevate it to a professional, government-grade administrative system with high-safety workflows.

**Architecture:** We will refine the design system tokens in the global CSS, group verbose table columns, replace risk-prone inline action buttons with discrete three-dots action menus (⋯), implement an autocomplete input for foreigner selection in forms, add escalation fields to the violation report, and ensure modal confirmations for all critical actions.

**Tech Stack:** ASP.NET Core MVC, Razor Pages, Bootstrap 5, Bootstrap Icons, JQuery, Vanilla CSS.

---

### Task 1: CSS Design System & Components Polish

**Files:**
- Modify: `wwwroot/css/police-layout.css`

**Step 1: Append refined badge, table action menu, autocomplete, and card modifier styles**

Add the following CSS rules to the end of `wwwroot/css/police-layout.css` to build the new design system components:
```css
/* Refined Badge System */
.police-badge {
    display: inline-block;
    padding: 6px 12px;
    font-size: 0.75rem;
    font-weight: 700;
    border-radius: 6px;
    border: 1px solid transparent;
    text-transform: uppercase;
    letter-spacing: 0.3px;
}
.police-badge.is-warning {
    background-color: #fffbeb !important;
    color: #b45309 !important;
    border-color: #fde68a !important;
}
.police-badge.is-success {
    background-color: #f0fdf4 !important;
    color: #15803d !important;
    border-color: #bbf7d0 !important;
}
.police-badge.is-danger {
    background-color: #fef2f2 !important;
    color: #b91c1c !important;
    border-color: #fecaca !important;
}
.police-badge.is-info {
    background-color: #f0f9ff !important;
    color: #0369a1 !important;
    border-color: #bae6fd !important;
}
.police-badge.is-neutral {
    background-color: #f8fafc !important;
    color: #475569 !important;
    border-color: #e2e8f0 !important;
}

/* Autocomplete Suggestion List */
.autocomplete-wrapper {
    position: relative;
}
.autocomplete-suggestions {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: white;
    border: 1px solid #cbd5e1;
    border-radius: 8px;
    max-height: 220px;
    overflow-y: auto;
    z-index: 1050;
    box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
    margin-top: 4px;
}
.autocomplete-suggestion {
    padding: 10px 14px;
    cursor: pointer;
    font-size: 0.85rem;
    transition: background 0.15s ease;
}
.autocomplete-suggestion:hover {
    background: #f1f5f9;
}
.autocomplete-suggestion.no-result {
    color: #64748b;
    cursor: default;
}

/* Style for action menus */
.dropdown-menu {
    border-color: #e2e8f0 !important;
    box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05), 0 4px 6px -4px rgba(0, 0, 0, 0.05) !important;
}
```

**Step 2: Commit**
```bash
git add wwwroot/css/police-layout.css
git commit -m "style: add refined badge, autocomplete, and dropdown styles"
```

---

### Task 2: Dashboard UI Refactoring

**Files:**
- Modify: `Views/CongAn/TongQuan.cshtml`

**Step 1: Apply semantic color cards, correct labels, and clean table actions**

Update `Views/CongAn/TongQuan.cshtml`:
- Replace label "NNN trong địa bàn" with "Người nước ngoài địa bàn"
- Replace label "Đã duyệt tạm trú" with "Đã duyệt hôm nay"
- Set background modifier colors for stat cards:
  - `is-warning` (Chờ duyệt): keep soft amber bg (`#fff8e1`).
  - `is-success` (Đã duyệt): set soft green bg (`#f0fdf4`, border color `#bbf7d0`).
  - `is-info-teal` (Người nước ngoài): set soft teal/blue bg (`#f0f9ff`, border color `#bae6fd`).
  - `is-neutral` (Cảnh báo đã gửi): set soft neutral/red bg (`#fef2f2`, border color `#fecaca`).
- Remove the direct inline "Duyệt" action buttons and forms from the pending table. Leave only the outline "Chi tiết" link button.

**Step 2: Verify compiling and commit**
```bash
dotnet build
git add Views/CongAn/TongQuan.cshtml
git commit -m "refactor: polish dashboard cards, labels, and table actions"
```

---

### Task 3: Phê duyệt khai báo (Declaration List & Detail Pages)

**Files:**
- Modify: `Views/CongAn/DanhSachKhaiBao.cshtml`
- Modify: `Views/CongAn/ChiTietKhaiBao.cshtml`

**Step 1: Group columns and replace list actions with a three-dots menu dropdown (⋯)**

Update `Views/CongAn/DanhSachKhaiBao.cshtml`:
- Group the columns into:
  - **Hồ sơ**: Mã hồ sơ + Ngày nộp.
  - **Người khai báo**: Họ tên + Số hộ chiếu + Quốc tịch (as badge).
  - **Lưu trú**: Địa chỉ + Cơ sở lưu trú (if applicable).
  - **Thời gian**: Từ ngày → Đến ngày.
  - **Trạng thái**
  - **Thao tác**: A single dropdown menu with a three-dots button `⋯`. Inside the menu:
    - "Xem chi tiết" (links to `ChiTietKhaiBao`)
    - "Phê duyệt nhanh" (submits approval form with confirmation, only visible for Pending)
    - "Từ chối nhanh" (triggers the quick reject modal, only visible for Pending)

**Step 2: Update detailed view for safety and header consistency**

Update `Views/CongAn/ChiTietKhaiBao.cshtml`:
- Replace card header `<div class="card-header bg-success ...">` with standard page header `.police-page-header`.
- Ensure Approve and Reject forms trigger confirmation prompts.

**Step 3: Verify compiling and commit**
```bash
dotnet build
git add Views/CongAn/DanhSachKhaiBao.cshtml Views/CongAn/ChiTietKhaiBao.cshtml
git commit -m "refactor: simplify declaration list table columns and implement action menu dropdown"
```

---

### Task 4: Territory Search Page Refactoring

**Files:**
- Modify: `Views/CongAn/TraCuu.cshtml`

**Step 1: Replace direct buttons with action dropdown menu and clean filter toggle**

Update `Views/CongAn/TraCuu.cshtml`:
- Replace direct warning and report buttons with a three-dots menu dropdown (`⋯`). Inside the dropdown:
  - "Gửi cảnh báo" (links to `CanhBaoViPham` with route parameter)
  - "Báo cáo XNC" (links to `BaoCaoViPham` with route parameter)
- Style the advanced filters toggle link to look like a clean small outline button `[ Bộ lọc nâng cao ]` using `btn btn-outline-secondary btn-xs text-success`.

**Step 2: Verify compiling and commit**
```bash
dotnet build
git add Views/CongAn/TraCuu.cshtml
git commit -m "refactor: simplify territory search table actions and polish filters"
```

---

### Task 5: Send Warning Form Autocomplete & Modal Refactor

**Files:**
- Modify: `Views/CongAn/CanhBaoViPham.cshtml`

**Step 1: Replace foreigner search + select dropdown with autocomplete input**

Update `Views/CongAn/CanhBaoViPham.cshtml`:
- Replace the search text input and select dropdown with an autocomplete search input.
- Write a JS suggestion engine that filters matching options from the preloaded list in `ViewBag.NguoiNuocNgoais` and shows suggestions inside a `.autocomplete-suggestions` list.
- When an item is selected, populate the hidden `MaNguoiNuocNgoai` field and update the dynamic profile information card on the right column.
- Display a neat empty state when no foreigner is selected.
- Add a confirmation modal that lists the selected foreigner's name, nationality, and details before submitting the warning.

**Step 2: Verify compiling and commit**
```bash
dotnet build
git add Views/CongAn/CanhBaoViPham.cshtml
git commit -m "refactor: integrate autocomplete and confirmation modal into send warning form"
```

---

### Task 6: escalation fields and Red Header Refactor on Report Page

**Files:**
- Modify: `Views/CongAn/BaoCaoViPham.cshtml`
- Modify: `Models/ViewModels/CongAn/BaoCaoViPhamViewModel.cs`
- Modify: `Controllers/CongAnController.cs`

**Step 1: Add new escalation fields in the view model**

Update `Models/ViewModels/CongAn/BaoCaoViPhamViewModel.cs` (or create if needed, wait, it already exists, verify fields first):
Add fields:
- `LoaiBaoCao` (Type of report, e.g., Hộ chiếu giả, Hoạt động bất hợp pháp, Visa quá hạn lâu ngày)
- `MucDoKhanCap` (Emergency level, e.g., Thấp, Trung bình, Khẩn cấp)
- `DeXuatXuLy` (Proposed handling / recommendation)

**Step 2: Update CongAnController to handle the new fields**

Update `Controllers/CongAnController.cs` inside `BaoCaoViPham` post action to map or validate these new fields if they are saved in a DB table. Since this project is MVC, make sure the fields are bound correctly:
```csharp
[HttpPost("report-violation")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> BaoCaoViPham(BaoCaoViPhamViewModel model)
```
Wait, we should inspect the entity and VM to make sure we don't break database schema. If the DB entity does not store these fields, we can include them in the VM and save them in the `GhiChu` or log, or if they are in the entity we map them. Let's inspect `BaoCaoViPhamViewModel.cs` first during execution.

**Step 3: Update View for autocomplete, fields, header, and confirmation modal**

Update `Views/CongAn/BaoCaoViPham.cshtml`:
- Replace the dark red header card with `.police-page-header` styled with a red border-left (`style="border-left: 4px solid #dc2626 !important;"`) and red text.
- Integrate the autocomplete search input for foreigner selection.
- Add input selects/textareas for the new escalation fields.
- Integrate a confirmation modal before escalation.

**Step 4: Verify compiling and commit**
```bash
dotnet build
git add Models/ViewModels/CongAn/BaoCaoViPhamViewModel.cs Controllers/CongAnController.cs Views/CongAn/BaoCaoViPham.cshtml
git commit -m "refactor: add escalation fields, autocomplete, and confirmation modal on report page"
```

---

### Task 7: Statistics Page Styling Refactoring

**Files:**
- Modify: `Views/CongAn/ThongKe.cshtml`

**Step 1: Replace hero gradient and soften progress bars**

Update `Views/CongAn/ThongKe.cshtml`:
- Replace the gradient hero card header block with standard page header `.police-page-header` (white bg, left green border).
- Style progress bars with clean light backgrounds and brand-fill colors instead of solid green stripes.
- Soften border and background highlights of stat blocks.

**Step 2: Verify compiling and commit**
```bash
dotnet build
git add Views/CongAn/ThongKe.cshtml
git commit -m "refactor: polish statistics view layout and progress indicators"
```

---

### Task 8: Verification & Compilation Test

**Step 1: Compile the entire solution**

Run command:
`dotnet build`
Expected: Build succeeded with 0 errors and 0 warnings.
