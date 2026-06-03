using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Accommodation;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.ChuLuuTru)]
public class AccommodationController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccommodationController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private async Task<CoSoLuuTru?> GetCurrentFacility()
    {
        var accountId = GetCurrentAccountId();
        return await _context.CoSoLuuTrus
            .FirstOrDefaultAsync(c => c.MaTaiKhoan == accountId);
    }

    // GET: Accommodation/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var facility = await GetCurrentFacility();
        if (facility == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với cơ sở lưu trú nào. Vui lòng liên hệ quản trị viên.";
            return RedirectToAction("Index", "Home");
        }

        var stays = await _context.LichSuLuuTrus
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru)
            .ToListAsync();

        var totalStaying = stays.Count(s => s.TrangThai == TrangThaiLuuTru.DangO);
        var totalStayed = stays.Count(s => s.TrangThai == TrangThaiLuuTru.DaRoi);
        var totalOverdue = stays.Count(s => s.TrangThai == TrangThaiLuuTru.QuaHan);

        var activeStaysList = await _context.LichSuLuuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru && l.TrangThai == TrangThaiLuuTru.DangO)
            .OrderByDescending(l => l.NgayBatDau)
            .Take(5)
            .ToListAsync();

        var recentCheckins = await _context.LichSuLuuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru)
            .OrderByDescending(l => l.NgayBatDau)
            .Take(5)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            CoSoInfo = facility,
            TotalStaying = totalStaying,
            TotalStayed = totalStayed,
            TotalOverdue = totalOverdue,
            ActiveStays = activeStaysList,
            RecentCheckins = recentCheckins
        };

        return View(viewModel);
    }

    // GET: Accommodation/KhaiBaoLuuTru
    public async Task<IActionResult> KhaiBaoLuuTru()
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        // Danh sách người nước ngoài hoạt động trên hệ thống để chủ cơ sở khai báo lưu trú cho họ
        var foreigners = await _context.NguoiNuocNgoais
            .Select(n => new SelectListItem
            {
                Value = n.MaNguoiNuocNgoai,
                Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
            }).ToListAsync();

        ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text");
        return View(new LuuTruViewModel());
    }

    // POST: Accommodation/KhaiBaoLuuTru
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhaiBaoLuuTru(LuuTruViewModel model)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        if (model.NgayKetThuc.HasValue && model.NgayKetThuc.Value <= model.NgayBatDau)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày rời đi dự kiến phải lớn hơn ngày bắt đầu");
        }

        if (!ModelState.IsValid)
        {
            var foreigners = await _context.NguoiNuocNgoais
                .Select(n => new SelectListItem
                {
                    Value = n.MaNguoiNuocNgoai,
                    Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
                }).ToListAsync();
            ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text");
            return View(model);
        }

        // Tạo bản ghi lưu trú
        var stay = new LichSuLuuTru
        {
            MaLSLuuTru = IdGenerator.NewMaLichSuLuuTru(_context),
            MaNguoiNuocNgoai = model.MaNguoiNuocNgoai,
            MaCoSoLuuTru = facility.MaCoSoLuuTru,
            NgayBatDau = model.NgayBatDau,
            NgayKetThuc = model.NgayKetThuc,
            Phong = model.Phong,
            TrangThai = TrangThaiLuuTru.DangO,
            GhiChu = model.GhiChu
        };

        _context.LichSuLuuTrus.Add(stay);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Khai báo lưu trú cho người nước ngoài thành công!";
        return RedirectToAction(nameof(DanhSachLuuTru));
    }

    // GET: Accommodation/DanhSachLuuTru
    public async Task<IActionResult> DanhSachLuuTru(string? search, int? page)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.LichSuLuuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru && 
                       (l.TrangThai == TrangThaiLuuTru.DangO || l.TrangThai == TrangThaiLuuTru.QuaHan));

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(l => l.NguoiNuocNgoai.HoTen.Contains(search) || 
                                     l.NguoiNuocNgoai.SoHoChieu.Contains(search) || 
                                     l.Phong.Contains(search));
        }

        ViewBag.Search = search;
        var pagedList = query.OrderByDescending(l => l.NgayBatDau).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // GET: Accommodation/LichSuLuuTru
    public async Task<IActionResult> LichSuLuuTru(string? search, int? page)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.LichSuLuuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(l => l.NguoiNuocNgoai.HoTen.Contains(search) || 
                                     l.NguoiNuocNgoai.SoHoChieu.Contains(search));
        }

        ViewBag.Search = search;
        var pagedList = query.OrderByDescending(l => l.NgayBatDau).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // POST: Accommodation/CapNhatTrangThai
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(string id, string actionType)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        var stay = await _context.LichSuLuuTrus
            .FirstOrDefaultAsync(l => l.MaLSLuuTru == id && l.MaCoSoLuuTru == facility.MaCoSoLuuTru);

        if (stay == null) return NotFound();

        if (actionType == "checkout")
        {
            stay.TrangThai = TrangThaiLuuTru.DaRoi;
            stay.NgayKetThuc = DateTime.Now;
            TempData["SuccessMessage"] = "Đã xác nhận người nước ngoài trả phòng (Check-out) thành công.";
        }
        else if (actionType == "overdue")
        {
            stay.TrangThai = TrangThaiLuuTru.QuaHan;
            TempData["WarningMessage"] = "Đã cập nhật trạng thái lưu trú thành Quá Hạn.";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(DanhSachLuuTru));
    }

    // GET: Accommodation/ThongTinCoSo
    public async Task<IActionResult> ThongTinCoSo()
    {
        var facility = await _context.CoSoLuuTrus
            .Include(c => c.TaiKhoan)
            .FirstOrDefaultAsync(c => c.MaTaiKhoan == GetCurrentAccountId());

        if (facility == null) return NotFound();

        return View(facility);
    }

    // GET: Accommodation/CapNhatCoSo
    public async Task<IActionResult> CapNhatCoSo()
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        var model = new CoSoViewModel
        {
            TenCoSo = facility.TenCoSo,
            DiaChi = facility.DiaChi,
            SoDienThoai = facility.SoDienThoai,
            Email = facility.Email
        };

        return View(model);
    }

    // POST: Accommodation/CapNhatCoSo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatCoSo(CoSoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        facility.TenCoSo = model.TenCoSo;
        facility.DiaChi = model.DiaChi;
        facility.SoDienThoai = model.SoDienThoai;
        facility.Email = model.Email;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin cơ sở lưu trú thành công!";
        return RedirectToAction(nameof(ThongTinCoSo));
    }
}
