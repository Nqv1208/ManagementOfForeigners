using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Officer;
using ManagementOfForeigners.Filters;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.CanBoXNC)]
public class OfficerController : Controller
{
    private readonly ApplicationDbContext _context;

    public OfficerController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    // GET: Officer/Dashboard
    public async Task<IActionResult> Dashboard()
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

        var viewModel = new DashboardViewModel
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

    // GET: Officer/TraCuu
    public async Task<IActionResult> TraCuu(string? search, string? quocTich, string? loaiVisa, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.NguoiNuocNgoais.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.HoTen.Contains(search) || 
                                     n.SoHoChieu.Contains(search) || 
                                     n.NoiCuTruHienTai.Contains(search));
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
        return View(pagedList);
    }

    // GET: Officer/ChiTietNguoiNuocNgoai
    public async Task<IActionResult> ChiTietNguoiNuocNgoai(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaNguoiNuocNgoai == id);

        if (foreigner == null) return NotFound();

        // Get residence declarations
        var declarations = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.CoSoLuuTru)
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Where(h => h.MaTaiKhoan == foreigner.MaTaiKhoan)
            .OrderByDescending(h => h.NgayKhaiBao)
            .ToListAsync();

        // Get stays in facilities
        var stays = await _context.LichSuLuuTrus
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
        var editHistories = await _context.LichSuCapNhatThongTins
            .Where(e => e.MaTaiKhoan == foreigner.MaTaiKhoan)
            .OrderByDescending(e => e.NgayCapNhat)
            .ToListAsync();

        ViewBag.Declarations = declarations;
        ViewBag.Stays = stays;
        ViewBag.Warnings = warnings;
        ViewBag.EditHistories = editHistories;

        return View(foreigner);
    }

    // GET: Officer/CanhBaoViPham
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

    // POST: Officer/CanhBaoViPham
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CanhBaoViPham(CanhBaoViewModel model)
    {
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
            MaCanBo = GetCurrentAccountId(),
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

    // GET: Officer/DanhSachCanhBao
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

    // GET: Officer/ThongKe
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
        var facilityGroup = await _context.LichSuLuuTrus
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
}
