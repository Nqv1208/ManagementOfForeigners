using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.QuanTri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.Admin)]
[Route("admin")]
public class QuanTriController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuanTriController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /admin/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> TongQuan()
    {
        var today = DateTime.Today;
        var next30Days = today.AddDays(30);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var model = new TongQuanViewModel
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

    // GET: /admin/accounts
    [HttpGet("accounts")]
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

    // POST: /admin/toggle-account-status/{id}
    [HttpPost("toggle-account-status/{id}")]
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

        if (account.TrangThai == TrangThaiTaiKhoan.ChoDuyet)
        {
            account.TrangThai = TrangThaiTaiKhoan.HoatDong;
            if (account.MaVaiTro == 2)
            {
                var chuCoSo = await _context.ChuCoSoLuuTrus.FirstOrDefaultAsync(c => c.MaTaiKhoan == account.MaTaiKhoan);
                if (chuCoSo != null)
                {
                    var facilities = await _context.CoSoLuuTrus.Where(f => f.MaChuCoSo == chuCoSo.MaChuCoSo).ToListAsync();
                    foreach (var facility in facilities)
                    {
                        facility.TrangThai = TrangThaiCoSo.DangHoatDong;
                    }
                }
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã phê duyệt tài khoản {account.TenDangNhap} và kích hoạt các cơ sở liên quan.";
        }
        else
        {
            account.TrangThai = account.TrangThai == TrangThaiTaiKhoan.Khoa
                ? TrangThaiTaiKhoan.HoatDong
                : TrangThaiTaiKhoan.Khoa;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái tài khoản {account.TenDangNhap}.";
        }

        return RedirectToAction(nameof(TaiKhoan));
    }

    // POST: /admin/reset-password/{id}
    [HttpPost("reset-password/{id}")]
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

    // GET: /admin/facilities
    [HttpGet("facilities")]
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

    // POST: /admin/toggle-facility-status/{id}
    [HttpPost("toggle-facility-status/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThaiCoSo(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var facility = await _context.CoSoLuuTrus.FirstOrDefaultAsync(c => c.MaCoSoLuuTru == id);
        if (facility == null) return NotFound();

        if (facility.TrangThai == TrangThaiCoSo.ChoDuyet)
        {
            facility.TrangThai = TrangThaiCoSo.DangHoatDong;
            var chuCoSo = await _context.ChuCoSoLuuTrus.FirstOrDefaultAsync(c => c.MaChuCoSo == facility.MaChuCoSo);
            if (chuCoSo != null)
            {
                var ownerAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == chuCoSo.MaTaiKhoan);
                if (ownerAccount != null && ownerAccount.TrangThai == TrangThaiTaiKhoan.ChoDuyet)
                {
                    ownerAccount.TrangThai = TrangThaiTaiKhoan.HoatDong;
                }
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã phê duyệt cơ sở {facility.TenCoSo} và kích hoạt tài khoản chủ sở hữu.";
        }
        else
        {
            facility.TrangThai = facility.TrangThai == TrangThaiCoSo.DangHoatDong
                ? TrangThaiCoSo.DaNgungHoatDong
                : TrangThaiCoSo.DangHoatDong;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái cơ sở {facility.TenCoSo}.";
        }

        return RedirectToAction(nameof(CoSoLuuTru));
    }

    // GET: /admin/reports
    [HttpGet("reports")]
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

    // POST: /admin/update-report-status
    [HttpPost("update-report-status")]
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

    // GET: /admin/wards
    [HttpGet("wards")]
    public async Task<IActionResult> DanhMucPhuongXa()
    {
        var wards = await _context.PhuongXas
            .OrderBy(p => p.MaPhuongXa)
            .ToListAsync();
        return View(wards);
    }

    // POST: /admin/wards/save
    [HttpPost("wards/save")]
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
            if (ward == null) return NotFound();
            ward.TenPhuongXa = tenPhuongXa.Trim();
            _context.PhuongXas.Update(ward);
            TempData["SuccessMessage"] = $"Đã cập nhật Phường/Xã: {tenPhuongXa}";
        }
        else
        {
            var maxId = await _context.PhuongXas.AnyAsync() ? await _context.PhuongXas.MaxAsync(p => p.MaPhuongXa) : 0;
            var ward = new PhuongXa
            {
                MaPhuongXa = maxId + 1,
                TenPhuongXa = tenPhuongXa.Trim()
            };
            _context.PhuongXas.Add(ward);
            TempData["SuccessMessage"] = $"Đã thêm mới Phường/Xã: {tenPhuongXa}";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(DanhMucPhuongXa));
    }

    // POST: /admin/wards/delete
    [HttpPost("wards/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaPhuongXa(int maPhuongXa)
    {
        var ward = await _context.PhuongXas
            .Include(p => p.CanBos)
            .Include(p => p.CoSoLuuTrus)
            .FirstOrDefaultAsync(p => p.MaPhuongXa == maPhuongXa);

        if (ward == null) return NotFound();

        if (ward.CanBos.Any() || ward.CoSoLuuTrus.Any())
        {
            TempData["ErrorMessage"] = "Không thể xóa Phường/Xã này vì có liên kết với cán bộ hoặc cơ sở lưu trú.";
            return RedirectToAction(nameof(DanhMucPhuongXa));
        }

        _context.PhuongXas.Remove(ward);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa Phường/Xã thành công.";
        return RedirectToAction(nameof(DanhMucPhuongXa));
    }

    // GET: /admin/permissions
    [HttpGet("permissions")]
    public async Task<IActionResult> QuanLyPhanQuyen()
    {
        var roles = await _context.VaiTros
            .Include(v => v.QuyenHans)
            .OrderBy(v => v.MaVaiTro)
            .ToListAsync();
        return View(roles);
    }

    // POST: /admin/permissions/add
    [HttpPost("permissions/add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemQuyen(int maVaiTro, string tenQuyen, string moTaQuyen)
    {
        if (string.IsNullOrWhiteSpace(tenQuyen))
        {
            TempData["ErrorMessage"] = "Tên quyền không được để trống.";
            return RedirectToAction(nameof(QuanLyPhanQuyen));
        }

        var roleExists = await _context.VaiTros.AnyAsync(v => v.MaVaiTro == maVaiTro);
        if (!roleExists) return NotFound();

        var permission = new QuyenHan
        {
            MaVaiTro = maVaiTro,
            TenQuyen = tenQuyen.Trim(),
            MoTaQuyen = moTaQuyen?.Trim() ?? string.Empty,
            NgayCapNhat = DateTime.Now
        };

        _context.QuyenHans.Add(permission);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Thêm quyền hạn thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    // POST: /admin/permissions/delete
    [HttpPost("permissions/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaQuyen(int maQuyen)
    {
        var permission = await _context.QuyenHans.FirstOrDefaultAsync(q => q.MaQuyen == maQuyen);
        if (permission == null) return NotFound();

        _context.QuyenHans.Remove(permission);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa quyền hạn thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    // POST: /admin/roles/update
    [HttpPost("roles/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatVaiTro(int maVaiTro, string moTaVaiTro, string trangThai)
    {
        var role = await _context.VaiTros.FirstOrDefaultAsync(v => v.MaVaiTro == maVaiTro);
        if (role == null) return NotFound();

        role.MoTaVaiTro = moTaVaiTro?.Trim() ?? string.Empty;
        role.TrangThai = trangThai ?? "Hoạt động";

        _context.VaiTros.Update(role);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Cập nhật vai trò thành công.";
        return RedirectToAction(nameof(QuanLyPhanQuyen));
    }

    // GET: /admin/activity-logs
    [HttpGet("activity-logs")]
    public async Task<IActionResult> NhatKyHoatDong()
    {
        // Fetch 100 recent updates
        var profileUpdates = await _context.LichSuCapNhatThongTinCaNhans
            .Include(u => u.TaiKhoan)
            .OrderByDescending(u => u.NgayCapNhat)
            .Take(100)
            .Select(u => new NhatKyHoatDongViewModel
            {
                OccurredAt = u.NgayCapNhat,
                ActorOrSource = u.TaiKhoan.TenDangNhap,
                Category = "Cập nhật thông tin",
                Detail = $"Thay đổi trường {u.TruongCapNhat}: '{u.GiaTriCu}' thành '{u.GiaTriMoi}'. Lý do: {u.LyDoCapNhat}",
                Status = "Đã cập nhật"
            })
            .ToListAsync();

        // Fetch 100 recent warnings
        var warnings = await _context.CanhBaoViPhams
            .Include(w => w.CanBo)
            .Include(w => w.NguoiNuocNgoai)
            .OrderByDescending(w => w.NgayCanhBao)
            .Take(100)
            .Select(w => new NhatKyHoatDongViewModel
            {
                OccurredAt = w.NgayCanhBao,
                ActorOrSource = w.CanBo.HoTen,
                Category = "Cảnh báo vi phạm",
                Detail = $"Gửi cảnh báo đến {w.NguoiNuocNgoai.HoTen} ({w.LoaiViPham}): {w.NoiDungCanhBao}",
                Status = w.TrangThai
            })
            .ToListAsync();

        // Fetch 100 recent reports
        var reports = await _context.BaoCaoViPhams
            .Include(r => r.CanBo)
            .Include(r => r.NguoiNuocNgoai)
            .OrderByDescending(r => r.NgayBaoCao)
            .Take(100)
            .Select(r => new NhatKyHoatDongViewModel
            {
                OccurredAt = r.NgayBaoCao,
                ActorOrSource = r.CanBo.HoTen,
                Category = "Báo cáo vi phạm",
                Detail = $"Báo cáo vi phạm của {r.NguoiNuocNgoai.HoTen}: {r.NoiDungBaoCao}",
                Status = r.TrangThaiXuLy
            })
            .ToListAsync();

        // Merge, sort, and display
        var allLogs = profileUpdates
            .Concat(warnings)
            .Concat(reports)
            .OrderByDescending(l => l.OccurredAt)
            .Take(200)
            .ToList();

        return View(allLogs);
    }
}
