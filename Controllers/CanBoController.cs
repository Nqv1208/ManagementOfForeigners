using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.CanBo;
using ManagementOfForeigners.Filters;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.CanBoXNC)]
[Route("officer")]
public class CanBoController : Controller
{
    private readonly ApplicationDbContext _context;

    public CanBoController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private async Task<CanBo?> GetCurrentCanBoAsync()
    {
        var accountId = GetCurrentAccountId();
        return await _context.CanBos.FirstOrDefaultAsync(c => c.MaTaiKhoan == accountId);
    }

    // GET: /officer/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> TongQuan()
    {
        var totalForeigners = await _context.NguoiNuocNgoais.CountAsync();
        var totalDeclarations = await _context.HoSoKhaiBaoTamTrus.CountAsync();
        var totalWarnings = await _context.CanhBaoViPhams.CountAsync();
        var totalReports = await _context.BaoCaoViPhams.CountAsync();

        var recentDeclarations = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .OrderByDescending(h => h.NgayKhaiBao)
            .Take(5)
            .ToListAsync();

        var recentWarnings = await _context.CanhBaoViPhams
            .Include(w => w.NguoiNuocNgoai)
            .OrderByDescending(w => w.NgayCanhBao)
            .Take(5)
            .ToListAsync();

        var recentReports = await _context.BaoCaoViPhams
            .Include(r => r.NguoiNuocNgoai)
            .Include(r => r.CanBo)
            .OrderByDescending(r => r.NgayBaoCao)
            .Take(5)
            .ToListAsync();

        var viewModel = new TongQuanViewModel
        {
            TotalForeigners = totalForeigners,
            TotalDeclarations = totalDeclarations,
            TotalWarnings = totalWarnings,
            TotalReports = totalReports,
            RecentDeclarations = recentDeclarations,
            RecentWarnings = recentWarnings,
            RecentReports = recentReports
        };

        return View(viewModel);
    }

    // GET: /officer/search
    [HttpGet("search")]
    public async Task<IActionResult> TraCuu(string? search, string? quocTich, string? loaiVisa, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.NguoiNuocNgoais.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.HoTen.Contains(search) || 
                                     n.SoHoChieu.Contains(search));
        }

        if (!string.IsNullOrEmpty(quocTich))
        {
            query = query.Where(n => n.QuocTich == quocTich);
        }

        if (!string.IsNullOrEmpty(loaiVisa))
        {
            query = query.Where(n => n.LoaiVisa == loaiVisa);
        }

        // Get list of nationalities and visas for filters
        ViewBag.Nationalities = await _context.NguoiNuocNgoais
            .Select(n => n.QuocTich)
            .Distinct()
            .ToListAsync();

        ViewBag.VisaTypes = await _context.NguoiNuocNgoais
            .Select(n => n.LoaiVisa)
            .Distinct()
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.QuocTich = quocTich;
        ViewBag.LoaiVisa = loaiVisa;

        var pagedList = query.OrderBy(n => n.HoTen).ToPagedList(pageNumber, pageSize);
        PopulateCurrentResidence(pagedList);
        return View(pagedList);
    }

    // GET: /officer/foreigner/{id}
    [HttpGet("foreigner/{id}")]
    public async Task<IActionResult> ChiTietNguoiNuocNgoai(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaNguoiNuocNgoai == id);

        if (foreigner == null) return NotFound();
        PopulateCurrentResidence(new[] { foreigner });

        // Get residence declarations
        var declarations = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.CoSoLuuTru)
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Where(h => h.MaTaiKhoan == foreigner.MaTaiKhoan)
            .OrderByDescending(h => h.NgayKhaiBao)
            .ToListAsync();

        // Get stays in facilities
        var stays = await _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.MaNguoiNuocNgoai == id)
            .OrderByDescending(l => l.NgayBatDau)
            .ToListAsync();

        // Get warnings
        var warnings = await _context.CanhBaoViPhams
            .Include(w => w.CanBo)
            .Where(w => w.MaNguoiNuocNgoai == id)
            .OrderByDescending(w => w.NgayCanhBao)
            .ToListAsync();

        // Get edit history
        var editHistories = await _context.LichSuCapNhatThongTinCaNhans
            .Where(e => e.MaTaiKhoan == foreigner.MaTaiKhoan)
            .OrderByDescending(e => e.NgayCapNhat)
            .ToListAsync();

        ViewBag.Declarations = declarations;
        ViewBag.Stays = stays;
        ViewBag.Warnings = warnings;
        ViewBag.EditHistories = editHistories;

        return View(foreigner);
    }

    // GET: /officer/send-warning
    [HttpGet("send-warning")]
    public async Task<IActionResult> CanhBaoViPham(string? maNguoiNuocNgoai)
    {
        var foreigners = await _context.NguoiNuocNgoais
            .Select(n => new SelectListItem
            {
                Value = n.MaNguoiNuocNgoai,
                Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
            }).ToListAsync();

        ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text", maNguoiNuocNgoai);
        
        var model = new CanhBaoViewModel
        {
            MaNguoiNuocNgoai = maNguoiNuocNgoai ?? string.Empty
        };

        return View(model);
    }

    // POST: /officer/send-warning
    [HttpPost("send-warning")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CanhBaoViPham(CanhBaoViewModel model)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        if (!ModelState.IsValid)
        {
            var foreigners = await _context.NguoiNuocNgoais
                .Select(n => new SelectListItem
                {
                    Value = n.MaNguoiNuocNgoai,
                    Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
                }).ToListAsync();
            ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text", model.MaNguoiNuocNgoai);
            return View(model);
        }

        var warning = new CanhBaoViPham
        {
            MaNguoiNuocNgoai = model.MaNguoiNuocNgoai,
            MaCanBo = canBo.MaCanBo,
            LoaiViPham = model.LoaiViPham,
            NoiDungCanhBao = model.NoiDungCanhBao,
            MucDoViPham = model.MucDoViPham,
            NgayCanhBao = DateTime.Now,
            TrangThai = TrangThaiCanhBao.DaGui,
            GhiChu = model.GhiChu
        };

        _context.CanhBaoViPhams.Add(warning);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gửi cảnh báo vi phạm cho người nước ngoài thành công!";
        return RedirectToAction(nameof(DanhSachCanhBao));
    }

    // GET: /officer/warnings
    [HttpGet("warnings")]
    public async Task<IActionResult> DanhSachCanhBao(string? search, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.CanhBaoViPhams
            .Include(c => c.NguoiNuocNgoai)
            .Include(c => c.CanBo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.NguoiNuocNgoai.HoTen.Contains(search) || 
                                     c.NguoiNuocNgoai.SoHoChieu.Contains(search) || 
                                     c.LoaiViPham.Contains(search));
        }

        ViewBag.Search = search;
        var pagedList = query.OrderByDescending(c => c.NgayCanhBao).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // GET: /officer/reports
    [HttpGet("reports")]
    public async Task<IActionResult> DanhSachBaoCao(string? search, string? trangThai, int? page)
    {
        var query = _context.BaoCaoViPhams
            .Include(r => r.NguoiNuocNgoai)
            .Include(r => r.CanBo)
            .ThenInclude(c => c.PhuongXa)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r =>
                r.NguoiNuocNgoai.HoTen.Contains(search) ||
                r.NguoiNuocNgoai.SoHoChieu.Contains(search) ||
                r.NoiDungBaoCao.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(r => r.TrangThaiXuLy == trangThai);
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        var pagedList = query.OrderByDescending(r => r.NgayBaoCao).ToPagedList(page ?? 1, 10);
        return View(pagedList);
    }

    // POST: /officer/update-report-status
    [HttpPost("update-report-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThaiBaoCao(string id, string trangThai)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var allowedStatuses = new[] { TrangThaiXuLyConst.ChuaXuLy, TrangThaiXuLyConst.DangXuLy, TrangThaiXuLyConst.DaXuLy };
        if (!allowedStatuses.Contains(trangThai))
        {
            TempData["ErrorMessage"] = "Trạng thái xử lý không hợp lệ.";
            return RedirectToAction(nameof(DanhSachBaoCao));
        }

        var report = await _context.BaoCaoViPhams.FirstOrDefaultAsync(r => r.MaBaoCao == id);
        if (report == null)
        {
            return NotFound();
        }

        report.TrangThaiXuLy = trangThai;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật trạng thái báo cáo vi phạm thành công.";
        return RedirectToAction(nameof(DanhSachBaoCao));
    }

    // GET: /officer/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> ThongKe()
    {
        var statistics = new Dictionary<string, int>();

        // Thống kê theo Quốc tịch
        var quocTichGroup = await _context.NguoiNuocNgoais
            .GroupBy(n => n.QuocTich)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        ViewBag.QuocTichStats = quocTichGroup;

        // Thống kê theo Loại Visa
        var visaGroup = await _context.NguoiNuocNgoais
            .GroupBy(n => n.LoaiVisa)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        ViewBag.VisaStats = visaGroup;

        // Thống kê cơ sở lưu trú có nhiều khách nhất
        var facilityGroup = await _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.TrangThai == TrangThaiLuuTru.DangO)
            .GroupBy(l => l.CoSoLuuTru.TenCoSo)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync();

        ViewBag.FacilityStats = facilityGroup;

        return View();
    }

    private void PopulateCurrentResidence(IEnumerable<NguoiNuocNgoai> foreigners)
    {
        var ids = foreigners.Select(f => f.MaNguoiNuocNgoai).ToList();
        if (ids.Count == 0)
        {
            return;
        }

        var residences = _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => ids.Contains(l.MaNguoiNuocNgoai) && l.TrangThai == TrangThaiLuuTru.DangO)
            .ToList()
            .GroupBy(l => l.MaNguoiNuocNgoai)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(l => l.NgayBatDau).First().CoSoLuuTru.DiaChi);

        foreach (var foreigner in foreigners)
        {
            foreigner.NoiCuTruHienTai = residences.TryGetValue(foreigner.MaNguoiNuocNgoai, out var address)
                ? address
                : "Chưa có dữ liệu cư trú hiện tại";
        }
    }
}
