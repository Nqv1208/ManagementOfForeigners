using System.Security.Claims;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new DashboardViewModel
        {
            ForeignerCount = await _context.NguoiNuocNgoais.CountAsync(),
            LodgingFacilityCount = await _context.CoSoLuuTrus.CountAsync(),
            DeclarationCount = await _context.HoSoKhaiBaoTamTrus.CountAsync(),
            UserCount = await _context.TaiKhoans.CountAsync(),
            WardCount = await _context.PhuongXas.CountAsync(),
            PendingReportCount = await _context.BaoCaoViPhams.CountAsync(r => r.TrangThaiXuLy != TrangThaiXuLyConst.DaXuLy)
        };

        return View(model);
    }

    public async Task<IActionResult> QuanLyTaiKhoan(string? search, int? maVaiTro, string? trangThai)
    {
        var query = _context.TaiKhoans
            .Include(t => t.VaiTro)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.TenDangNhap.Contains(search) ||
                (t.Email != null && t.Email.Contains(search)) ||
                (t.SoDienThoai != null && t.SoDienThoai.Contains(search)));
        }

        if (maVaiTro.HasValue)
        {
            query = query.Where(t => t.MaVaiTro == maVaiTro.Value);
        }

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(t => t.TrangThai == trangThai);
        }

        await LoadRoleLookupsAsync(maVaiTro);
        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(t => t.NgayTao).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTaiKhoan(string maTaiKhoan, int maVaiTro, string trangThai)
    {
        var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);
        if (account == null)
        {
            return NotFound();
        }

        var currentAccountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (account.MaTaiKhoan == currentAccountId && trangThai == TrangThaiTaiKhoan.Khoa)
        {
            TempData["ErrorMessage"] = "Quản trị viên không thể tự khóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(QuanLyTaiKhoan));
        }

        if (!await _context.VaiTros.AnyAsync(v => v.MaVaiTro == maVaiTro))
        {
            TempData["ErrorMessage"] = "Vai trò không hợp lệ.";
            return RedirectToAction(nameof(QuanLyTaiKhoan));
        }

        var validStatuses = new[] { TrangThaiTaiKhoan.HoatDong, TrangThaiTaiKhoan.Khoa, TrangThaiTaiKhoan.ChoDuyet };
        if (!validStatuses.Contains(trangThai))
        {
            TempData["ErrorMessage"] = "Trạng thái tài khoản không hợp lệ.";
            return RedirectToAction(nameof(QuanLyTaiKhoan));
        }

        account.MaVaiTro = maVaiTro;
        account.TrangThai = trangThai;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
        return RedirectToAction(nameof(QuanLyTaiKhoan));
    }

    public async Task<IActionResult> QuanLyPhanQuyen()
    {
        var vaiTros = await _context.VaiTros
            .Include(v => v.QuyenHans.OrderBy(q => q.TenQuyen))
            .OrderBy(v => v.MaVaiTro)
            .ToListAsync();

        return View(vaiTros);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatVaiTro(int maVaiTro, string moTaVaiTro, string trangThai)
    {
        var role = await _context.VaiTros.FirstOrDefaultAsync(v => v.MaVaiTro == maVaiTro);
        if (role == null)
        {
            return NotFound();
        }

        role.MoTaVaiTro = moTaVaiTro;
        role.TrangThai = trangThai;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật vai trò thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemQuyen(int maVaiTro, string tenQuyen, string? moTaQuyen)
    {
        if (string.IsNullOrWhiteSpace(tenQuyen))
        {
            TempData["ErrorMessage"] = "Tên quyền không được để trống.";
            return RedirectToAction(nameof(QuanLyPhanQuyen));
        }

        if (!await _context.VaiTros.AnyAsync(v => v.MaVaiTro == maVaiTro))
        {
            return NotFound();
        }

        _context.QuyenHans.Add(new QuyenHan
        {
            MaVaiTro = maVaiTro,
            TenQuyen = tenQuyen.Trim(),
            MoTaQuyen = moTaQuyen?.Trim() ?? string.Empty,
            NgayCapNhat = DateTime.Now
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Thêm quyền cho vai trò thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaQuyen(int maQuyen)
    {
        var permission = await _context.QuyenHans.FindAsync(maQuyen);
        if (permission == null)
        {
            return NotFound();
        }

        _context.QuyenHans.Remove(permission);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa quyền thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    public async Task<IActionResult> DanhMucPhuongXa()
    {
        var wards = await _context.PhuongXas
            .OrderBy(p => p.TenPhuongXa)
            .ToListAsync();

        return View(wards);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuPhuongXa(int? maPhuongXa, string tenPhuongXa)
    {
        if (string.IsNullOrWhiteSpace(tenPhuongXa))
        {
            TempData["ErrorMessage"] = "Tên Phường/Xã không được để trống.";
            return RedirectToAction(nameof(DanhMucPhuongXa));
        }

        if (maPhuongXa.HasValue)
        {
            var ward = await _context.PhuongXas.FirstOrDefaultAsync(p => p.MaPhuongXa == maPhuongXa.Value);
            if (ward == null)
            {
                return NotFound();
            }

            ward.TenPhuongXa = tenPhuongXa.Trim();
        }
        else
        {
            var nextId = await _context.PhuongXas.Select(p => (int?)p.MaPhuongXa).MaxAsync() ?? 0;
            _context.PhuongXas.Add(new PhuongXa
            {
                MaPhuongXa = nextId + 1,
                TenPhuongXa = tenPhuongXa.Trim()
            });
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Lưu danh mục Phường/Xã thành công.";
        return RedirectToAction(nameof(DanhMucPhuongXa));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaPhuongXa(int maPhuongXa)
    {
        var ward = await _context.PhuongXas
            .Include(p => p.CanBos)
            .Include(p => p.CoSoLuuTrus)
            .FirstOrDefaultAsync(p => p.MaPhuongXa == maPhuongXa);
        if (ward == null)
        {
            return NotFound();
        }

        if (ward.CanBos.Any() || ward.CoSoLuuTrus.Any())
        {
            TempData["ErrorMessage"] = "Không thể xóa Phường/Xã đang gắn với cán bộ hoặc cơ sở lưu trú.";
            return RedirectToAction(nameof(DanhMucPhuongXa));
        }

        _context.PhuongXas.Remove(ward);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa Phường/Xã thành công.";
        return RedirectToAction(nameof(DanhMucPhuongXa));
    }

    public async Task<IActionResult> NhatKyHoatDong()
    {
        var personalUpdates = await _context.LichSuCapNhatThongTinCaNhans
            .Include(l => l.TaiKhoan)
            .OrderByDescending(l => l.NgayCapNhat)
            .Take(30)
            .Select(l => new AuditEntryViewModel
            {
                OccurredAt = l.NgayCapNhat,
                ActorOrSource = l.TaiKhoan.TenDangNhap,
                Category = "Cập nhật thông tin cá nhân",
                Detail = $"{l.TruongCapNhat}: {l.GiaTriCu ?? "(trống)"} -> {l.GiaTriMoi}",
                Status = l.TrangThai ?? "Đã ghi nhận"
            })
            .ToListAsync();

        var warnings = await _context.CanhBaoViPhams
            .Include(w => w.NguoiNuocNgoai)
            .Include(w => w.CanBo)
            .OrderByDescending(w => w.NgayCanhBao)
            .Take(30)
            .Select(w => new AuditEntryViewModel
            {
                OccurredAt = w.NgayCanhBao,
                ActorOrSource = w.CanBo.HoTen,
                Category = "Cảnh báo vi phạm",
                Detail = $"{w.NguoiNuocNgoai.HoTen} - {w.LoaiViPham}",
                Status = w.TrangThai
            })
            .ToListAsync();

        var reports = await _context.BaoCaoViPhams
            .Include(r => r.NguoiNuocNgoai)
            .Include(r => r.CanBo)
            .OrderByDescending(r => r.NgayBaoCao)
            .Take(30)
            .Select(r => new AuditEntryViewModel
            {
                OccurredAt = r.NgayBaoCao,
                ActorOrSource = r.CanBo.HoTen,
                Category = "Báo cáo vi phạm",
                Detail = $"{r.NguoiNuocNgoai.HoTen} - {r.NoiDungBaoCao}",
                Status = r.TrangThaiXuLy
            })
            .ToListAsync();

        var model = personalUpdates
            .Concat(warnings)
            .Concat(reports)
            .OrderByDescending(e => e.OccurredAt)
            .Take(50)
            .ToList();

        return View(model);
    }

    private async Task LoadRoleLookupsAsync(int? selectedRoleId = null)
    {
        var roles = await _context.VaiTros.OrderBy(v => v.MaVaiTro).ToListAsync();
        ViewBag.VaiTros = new SelectList(roles, "MaVaiTro", "TenVaiTro", selectedRoleId);
    }
}
