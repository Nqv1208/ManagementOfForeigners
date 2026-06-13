using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
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
        var today = DateTime.Today;
        var next30Days = today.AddDays(30);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var model = new AdminDashboardViewModel
        {
            TotalAccounts = await _context.TaiKhoans.CountAsync(),
            ActiveAccounts = await _context.TaiKhoans.CountAsync(t => t.TrangThai == TrangThaiTaiKhoan.HoatDong),
            LockedAccounts = await _context.TaiKhoans.CountAsync(t => t.TrangThai == TrangThaiTaiKhoan.Khoa),
            NewAccountsThisMonth = await _context.TaiKhoans.CountAsync(t => t.NgayTao >= monthStart),

            TotalForeigners = await _context.NguoiNuocNgoais.CountAsync(),
            VisaExpiring30Days = await _context.NguoiNuocNgoais.CountAsync(n => n.NgayHetHanVisa >= today && n.NgayHetHanVisa <= next30Days),
            TotalFacilities = await _context.CoSoLuuTrus.CountAsync(),
            ActiveFacilities = await _context.CoSoLuuTrus.CountAsync(c => c.TrangThai == TrangThaiCoSo.DangHoatDong),

            PendingDeclarations = await _context.HoSoKhaiBaoTamTrus.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet),
            ApprovedDeclarations = await _context.HoSoKhaiBaoTamTrus.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.DaDuyet),
            RejectedDeclarations = await _context.HoSoKhaiBaoTamTrus.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.TuChoi),

            TotalWarnings = await _context.CanhBaoViPhams.CountAsync(),
            OpenReports = await _context.BaoCaoViPhams.CountAsync(r => r.TrangThaiXuLy != TrangThaiXuLyConst.DaXuLy)
        };

        model.RoleStats = await _context.TaiKhoans
            .Include(t => t.VaiTro)
            .GroupBy(t => t.VaiTro.TenVaiTro)
            .Select(g => new AdminStatItemViewModel { Label = g.Key, Value = g.Count() })
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        model.DeclarationStats = await _context.HoSoKhaiBaoTamTrus
            .GroupBy(h => h.TrangThai)
            .Select(g => new AdminStatItemViewModel { Label = g.Key, Value = g.Count() })
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        model.LatestAccounts = await BuildAccountQuery()
            .OrderByDescending(t => t.NgayTao)
            .Take(8)
            .Select(AccountRowProjection)
            .ToListAsync();

        model.LatestDeclarations = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
                .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .OrderByDescending(h => h.NgayKhaiBao)
            .Take(8)
            .Select(h => new AdminDeclarationRowViewModel
            {
                MaHSKhaiBao = h.MaHSKhaiBao,
                HoTenNguoiNuocNgoai = h.TaiKhoan.NguoiNuocNgoai != null ? h.TaiKhoan.NguoiNuocNgoai.HoTen : "—",
                SoHoChieu = h.TaiKhoan.NguoiNuocNgoai != null ? h.TaiKhoan.NguoiNuocNgoai.SoHoChieu : "—",
                DiaChiLuuTru = h.DiaChiLuuTru,
                TenCoSo = h.CoSoLuuTru != null ? h.CoSoLuuTru.TenCoSo : null,
                NgayKhaiBao = h.NgayKhaiBao,
                NgayBatDau = h.NgayBatDau,
                NgayKetThuc = h.NgayKetThuc,
                TrangThai = h.TrangThai
            })
            .ToListAsync();

        model.LatestReports = await BuildReportQuery()
            .OrderByDescending(r => r.NgayBaoCao)
            .Take(8)
            .ToListAsync();

        return View(model);
    }

    public async Task<IActionResult> TaiKhoan(string? search, int? role, string? status)
    {
        var query = BuildAccountQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(t => t.TenDangNhap.Contains(search)
                || (t.Email != null && t.Email.Contains(search))
                || (t.SoDienThoai != null && t.SoDienThoai.Contains(search))
                || (t.NguoiNuocNgoai != null && t.NguoiNuocNgoai.HoTen.Contains(search))
                || (t.ChuCoSoLuuTru != null && (t.ChuCoSoLuuTru.HoTen.Contains(search) || t.ChuCoSoLuuTru.CoSoLuuTrus.Any(c => c.TenCoSo.Contains(search))))
                || (t.CanBo != null && t.CanBo.HoTen.Contains(search)));
        }

        if (role.HasValue)
        {
            query = query.Where(t => t.MaVaiTro == role.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.TrangThai == status);
        }

        ViewBag.Search = search;
        ViewBag.Role = role;
        ViewBag.Status = status;
        ViewBag.Roles = await _context.VaiTros.OrderBy(v => v.MaVaiTro).ToListAsync();

        var accounts = await query
            .OrderBy(t => t.MaVaiTro)
            .ThenByDescending(t => t.NgayTao)
            .Take(200)
            .Select(AccountRowProjection)
            .ToListAsync();

        return View(accounts);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThaiTaiKhoan(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var currentAccountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == currentAccountId)
        {
            TempData["ErrorMessage"] = "Không thể khoá chính tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(TaiKhoan));
        }

        var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
        if (account == null) return NotFound();

        account.TrangThai = account.TrangThai == TrangThaiTaiKhoan.Khoa
            ? TrangThaiTaiKhoan.HoatDong
            : TrangThaiTaiKhoan.Khoa;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái tài khoản {account.TenDangNhap}.";
        return RedirectToAction(nameof(TaiKhoan));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
        if (account == null) return NotFound();

        var tempPassword = $"Tmp@{RandomNumberGenerator.GetInt32(100000, 999999)}";
        account.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
        await _context.SaveChangesAsync();

        TempData["InfoMessage"] = $"Mật khẩu tạm thời của {account.TenDangNhap}: {tempPassword}. Yêu cầu người dùng đổi lại sau khi đăng nhập.";
        return RedirectToAction(nameof(TaiKhoan));
    }

    public async Task<IActionResult> CoSoLuuTru(string? search, string? status)
    {
        var query = _context.CoSoLuuTrus
            .Include(c => c.ChuCoSoLuuTru)
                .ThenInclude(cc => cc.TaiKhoan)
            .Include(c => c.LichSuCuTrus)
            .Include(c => c.HoSoKhaiBaoTamTrus)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(c => c.TenCoSo.Contains(search)
                || c.DiaChi.Contains(search)
                || c.Email.Contains(search)
                || c.SoDienThoai.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c => c.TrangThai == status);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;

        var facilities = await query
            .OrderBy(c => c.TrangThai)
            .ThenBy(c => c.TenCoSo)
            .Select(c => new AdminFacilityRowViewModel
            {
                MaCoSoLuuTru = c.MaCoSoLuuTru,
                TenCoSo = c.TenCoSo,
                DiaChi = c.DiaChi,
                SoDienThoai = c.SoDienThoai,
                Email = c.Email,
                TrangThai = c.TrangThai,
                TenDangNhap = c.ChuCoSoLuuTru.TaiKhoan.TenDangNhap,
                ChuCoSo = c.ChuCoSoLuuTru.HoTen,
                SoKhachDangO = c.LichSuCuTrus.Count(l => l.TrangThai == TrangThaiLuuTru.DangO),
                TongHoSoKhaiBao = c.HoSoKhaiBaoTamTrus.Count
            })
            .Take(200)
            .ToListAsync();

        return View(facilities);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThaiCoSo(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var facility = await _context.CoSoLuuTrus.FirstOrDefaultAsync(c => c.MaCoSoLuuTru == id);
        if (facility == null) return NotFound();

        facility.TrangThai = facility.TrangThai == TrangThaiCoSo.DangHoatDong
            ? TrangThaiCoSo.DaNgungHoatDong
            : TrangThaiCoSo.DangHoatDong;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái cơ sở {facility.TenCoSo}.";
        return RedirectToAction(nameof(CoSoLuuTru));
    }

    public async Task<IActionResult> BaoCaoViPham(string? status)
    {
        var query = BuildReportQuery();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.TrangThaiXuLy == status);
        }

        ViewBag.Status = status;

        var reports = await query
            .OrderByDescending(r => r.NgayBaoCao)
            .Take(200)
            .ToListAsync();

        return View(reports);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThaiBaoCao(string id, string trangThai)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var allowed = new[]
        {
            TrangThaiXuLyConst.ChuaXuLy,
            TrangThaiXuLyConst.DangXuLy,
            TrangThaiXuLyConst.DaXuLy
        };

        if (!allowed.Contains(trangThai))
        {
            TempData["ErrorMessage"] = "Trạng thái xử lý không hợp lệ.";
            return RedirectToAction(nameof(BaoCaoViPham));
        }

        var report = await _context.BaoCaoViPhams.FirstOrDefaultAsync(r => r.MaBaoCao == id);
        if (report == null) return NotFound();

        report.TrangThaiXuLy = trangThai;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái báo cáo {report.MaBaoCao}.";
        return RedirectToAction(nameof(BaoCaoViPham));
    }

    private IQueryable<TaiKhoan> BuildAccountQuery()
    {
        return _context.TaiKhoans
            .Include(t => t.VaiTro)
            .Include(t => t.NguoiNuocNgoai)
            .Include(t => t.CanBo)
            .Include(t => t.ChuCoSoLuuTru)
                .ThenInclude(c => c!.CoSoLuuTrus)
            .AsQueryable();
    }

    private static readonly Expression<Func<TaiKhoan, AdminAccountRowViewModel>> AccountRowProjection = t => new AdminAccountRowViewModel
    {
            MaTaiKhoan = t.MaTaiKhoan,
            TenDangNhap = t.TenDangNhap,
            VaiTro = t.VaiTro.TenVaiTro,
            Email = t.Email,
            SoDienThoai = t.SoDienThoai,
            TrangThai = t.TrangThai,
            NgayTao = t.NgayTao,
            LanDangNhapCuoi = t.LanDangNhapCuoi,
            OwnerDisplayName = t.NguoiNuocNgoai != null ? t.NguoiNuocNgoai.HoTen
                : t.CanBo != null ? t.CanBo.HoTen
                : t.ChuCoSoLuuTru != null ? t.ChuCoSoLuuTru.HoTen
                : "—"
    };

    private IQueryable<AdminReportRowViewModel> BuildReportQuery()
    {
        return _context.BaoCaoViPhams
            .Include(r => r.NguoiNuocNgoai)
            .Include(r => r.CanBo)
            .Select(r => new AdminReportRowViewModel
            {
                MaBaoCao = r.MaBaoCao,
                HoTenNguoiNuocNgoai = r.NguoiNuocNgoai.HoTen,
                SoHoChieu = r.NguoiNuocNgoai.SoHoChieu,
                NoiDungBaoCao = r.NoiDungBaoCao,
                NgayBaoCao = r.NgayBaoCao,
                CanBoBaoCao = r.CanBo.HoTen,
                TrangThaiXuLy = r.TrangThaiXuLy
            });
    }
}
