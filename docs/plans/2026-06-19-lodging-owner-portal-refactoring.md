# Lodging Facility Owner Portal Refactoring Implementation Plan

**Goal:** Rebuild the lodging facility owner portal (`CoSoLuuTru` role) from a public-style header/footer look to a dedicated, premium administrative portal dashboard (using a left sidebar, top status bars, and streamlined data views), similar to the police/admin portals but tailored visually to accommodation providers.

**Key Architecture & UI Elements:**
1. **Layout**: Create a custom layout file `Views/Shared/_CoSoLuuTruLayout.cshtml` with a sticky left sidebar and a header topbar displaying the active lodging facility profile.
2. **Theme**: Apply a sleek, professional theme combining dark slate/grey for the sidebar (`#0f172a` / `#1e293b`) and forest green/emerald accents (`#2f8f1f`) to reflect both commercial lodging and official portal alignment.
3. **Scoped Application**: Place a localized `Views/CoSoLuuTru/_ViewStart.cshtml` so all lodging views automatically apply the new layout without intrusive page edits.
4. **Refined Views**: Standardize page headers (using `.coso-page-header`), card elements, tables, action forms, and badges in the 6 lodging views to match the portal design density and aesthetics.

---

### Task 1: Create Scoped Assets (CSS, JS, and ViewStart)

**Files:**
- Create: `wwwroot/css/cosoluutru-layout.css`
- Create: `wwwroot/js/cosoluutru-layout.js`
- Create: `Views/CoSoLuuTru/_ViewStart.cshtml`

**Details:**
- Define layout structure styles: sidebar widths, flex wrapper margins, colors, animations, responsive controls, page header layouts (`.coso-page-header`), and table alignment.
- Implement sidebar menu toggle script in `cosoluutru-layout.js` to manage mobile views.
- Configure `_ViewStart.cshtml` under the lodging views folder to target `_CoSoLuuTruLayout`.

---

### Task 2: Create the Portal Layout File

**Files:**
- Create: `Views/Shared/_CoSoLuuTruLayout.cshtml`

**Details:**
- Inject `ApplicationDbContext` to query and cache the logged-in user's facility name and status dynamically.
- Draw sidebar menu containing:
  - **TongQuan** (Bảng điều khiển)
  - **KhaiBaoLuuTru** (Khai báo lưu trú mới)
  - **DanhSachLuuTru** (Khách đang lưu trú)
  - **LichSuCuTru** (Lịch sử lưu trú)
  - **ThongTinCoSo** (Thông tin cơ sở)
- Include user profile details (Owner name, Facility name) and a clean "Đăng xuất" form in the topbar header.
- Add success/error/warning alerts toast handlers based on `TempData`.

---

### Task 3: Refactor the Dashboard View (TongQuan.cshtml)

**Files:**
- Modify: `Views/CoSoLuuTru/TongQuan.cshtml`

**Details:**
- Replace the high-contrast public welcome header card with a clean, professional page header displaying facility name, address, and an operational status badge.
- Re-style stat cards using lighter backgrounds, border-left highlights, and matching icons.
- Polish the action blocks and the recent guests list table to align with portal styling.

---

### Task 4: Refactor the Guest List Views (DanhSachLuuTru.cshtml & LichSuCuTru.cshtml)

**Files:**
- Modify: `Views/CoSoLuuTru/DanhSachLuuTru.cshtml`
- Modify: `Views/CoSoLuuTru/LichSuCuTru.cshtml`

**Details:**
- Remove the old `.gov-card-header` class with dark background fills. Use clear `.coso-page-header` titles and standard secondary white cards.
- Restructure tables: clean lines, light badges, and uniform column paddings.
- Standardize actions: replace raw form layouts with properly aligned button grids or dropdown menus for Check-out and Overdue updates.

---

### Task 5: Refactor the Facility Profile Views (ThongTinCoSo.cshtml & CapNhatCoSo.cshtml)

**Files:**
- Modify: `Views/CoSoLuuTru/ThongTinCoSo.cshtml`
- Modify: `Views/CoSoLuuTru/CapNhatCoSo.cshtml`

**Details:**
- Redesign the profile details display: use clean information grids, consistent Bootstrap icons, and modern label hierarchies.
- Refactor the update facility form fields with standard spacing, input group icons, and responsive layouts.

---

### Task 6: Refactor the Declaration Check-In Form (KhaiBaoLuuTru.cshtml)

**Files:**
- Modify: `Views/CoSoLuuTru/KhaiBaoLuuTru.cshtml`

**Details:**
- Adapt the styling of the declaration form page to match the new layout container.
- Clean up any legacy or overlapping wrapper classes.
- Ensure validation scripts, interactive date pickers, and async passport lookups function correctly.

---

### Task 7: Build & Validate

**Details:**
- Compile the solution with `dotnet build`.
- Verify views load successfully and render matching portal components.
