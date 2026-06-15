using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.CoSoLuuTru;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.ChuLuuTru)]
[Route("accommodation")]
public class CoSoLuuTruController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoSoLuuTruController(ApplicationDbContext context)
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
            .Include(c => c.PhuongXa)
            .Include(c => c.ChuCoSoLuuTru)
            .FirstOrDefaultAsync(c => c.ChuCoSoLuuTru.MaTaiKhoan == accountId);
    }

    // GET: /accommodation/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> TongQuan()
    {
        var facility = await GetCurrentFacility();
        if (facility == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với cơ sở lưu trú nào. Vui lòng liên hệ quản trị viên.";
            return RedirectToAction("Index", "TrangChu");
        }

        var stays = await _context.LichSuCuTrus
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru)
            .ToListAsync();

        var totalStaying = stays.Count(s => s.TrangThai == TrangThaiLuuTru.DangO);
        var totalStayed = stays.Count(s => s.TrangThai == TrangThaiLuuTru.DaRoi);
        var totalOverdue = stays.Count(s => s.TrangThai == TrangThaiLuuTru.QuaHan);

        var activeStaysList = await _context.LichSuCuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru && l.TrangThai == TrangThaiLuuTru.DangO)
            .OrderByDescending(l => l.NgayBatDau)
            .Take(5)
            .ToListAsync();

        var recentCheckins = await _context.LichSuCuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru)
            .OrderByDescending(l => l.NgayBatDau)
            .Take(5)
            .ToListAsync();

        var viewModel = new TongQuanViewModel
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

    // GET: /accommodation/declare
    [HttpGet("declare")]
    public async Task<IActionResult> KhaiBaoLuuTru()
    {
        var facility = await GetCurrentFacility();
        if (facility == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với cơ sở lưu trú nào.";
            return RedirectToAction(nameof(TongQuan));
        }

        var viewModel = new LuuTruCreateViewModel
        {
            MaCoSoLuuTru = facility.MaCoSoLuuTru,
            TenCoSoLuuTru = facility.TenCoSo,
            DiaChiCoSo = facility.DiaChi,
            PhuongXa = facility.PhuongXa?.TenPhuongXa ?? string.Empty,
            NguoiDaiDien = facility.ChuCoSoLuuTru?.HoTen ?? string.Empty,
            SoDienThoaiCoSo = facility.SoDienThoai,
            CoSoDaDuocDuyet = facility.TrangThai == TrangThaiCoSo.DangHoatDong,
            NgayBatDau = DateTime.Today
        };

        return View(viewModel);
    }

    // POST: /accommodation/declare
    [HttpPost("declare")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhaiBaoLuuTru(LuuTruCreateViewModel model)
    {
        var facility = await GetCurrentFacility();
        if (facility == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với cơ sở lưu trú nào.";
            return RedirectToAction(nameof(TongQuan));
        }

        // Điền lại thông tin cơ sở phục vụ hiển thị khi form có lỗi
        model.MaCoSoLuuTru = facility.MaCoSoLuuTru;
        model.TenCoSoLuuTru = facility.TenCoSo;
        model.DiaChiCoSo = facility.DiaChi;
        model.PhuongXa = facility.PhuongXa?.TenPhuongXa ?? string.Empty;
        model.NguoiDaiDien = facility.ChuCoSoLuuTru?.HoTen ?? string.Empty;
        model.SoDienThoaiCoSo = facility.SoDienThoai;
        model.CoSoDaDuocDuyet = facility.TrangThai == TrangThaiCoSo.DangHoatDong;

        if (!model.CoSoDaDuocDuyet)
        {
            ModelState.AddModelError(string.Empty, "Cơ sở lưu trú chưa được duyệt, không thể gửi khai báo.");
        }

        if (model.NgayKetThucDuKien.HasValue && model.NgayKetThucDuKien.Value < model.NgayBatDau)
        {
            ModelState.AddModelError(nameof(model.NgayKetThucDuKien), "Ngày rời đi dự kiến phải lớn hơn hoặc bằng ngày bắt đầu.");
        }

        if (model.MucDichLuuTru == "Khác" && string.IsNullOrWhiteSpace(model.MucDichKhac))
        {
            ModelState.AddModelError(nameof(model.MucDichKhac), "Mô tả mục đích khác không được để trống.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Tìm hoặc tạo/cập nhật thông tin người nước ngoài theo số hộ chiếu
        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.SoHoChieu == model.SoHoChieu.Trim());

        if (foreigner == null)
        {
            foreigner = new NguoiNuocNgoai
            {
                MaNguoiNuocNgoai = IdGenerator.NewMaNguoiNuocNgoai(_context),
                MaTaiKhoan = null!,
                HoTen = model.HoTen.Trim(),
                NgaySinh = model.NgaySinh ?? DateTime.Today,
                GioiTinh = model.GioiTinh,
                QuocTich = model.QuocTich.Trim(),
                SoHoChieu = model.SoHoChieu.Trim(),
                NgayCapHoChieu = model.NgayCapHoChieu ?? DateTime.Today,
                NgayHetHanHoChieu = model.NgayHetHanHoChieu ?? DateTime.Today.AddYears(5),
                LoaiVisa = model.LoaiGiayToCuTru,
                NgayHetHanVisa = model.NgayHetHanHoChieu ?? DateTime.Today.AddYears(5)
            };
            _context.NguoiNuocNgoais.Add(foreigner);
            await _context.SaveChangesAsync();
        }
        else
        {
            foreigner.HoTen = model.HoTen.Trim();
            foreigner.NgaySinh = model.NgaySinh ?? foreigner.NgaySinh;
            foreigner.GioiTinh = model.GioiTinh;
            foreigner.QuocTich = model.QuocTich.Trim();
            if (model.NgayCapHoChieu.HasValue) foreigner.NgayCapHoChieu = model.NgayCapHoChieu.Value;
            if (model.NgayHetHanHoChieu.HasValue) foreigner.NgayHetHanHoChieu = model.NgayHetHanHoChieu.Value;
            foreigner.LoaiVisa = model.LoaiGiayToCuTru;
            if (model.NgayHetHanHoChieu.HasValue) foreigner.NgayHetHanVisa = model.NgayHetHanHoChieu.Value;
            
            _context.NguoiNuocNgoais.Update(foreigner);
            await _context.SaveChangesAsync();
        }

        // Tạo bản ghi LichSuCuTru
        var stay = new LichSuCuTru
        {
            MaLSLuuTru = IdGenerator.NewMaLichSuCuTru(_context),
            MaNguoiNuocNgoai = foreigner.MaNguoiNuocNgoai,
            MaCoSoLuuTru = facility.MaCoSoLuuTru,
            NgayBatDau = model.NgayBatDau,
            NgayKetThuc = model.NgayKetThucDuKien,
            Phong = model.SoPhong.Trim(),
            TrangThai = TrangThaiLuuTru.DangO,
            GhiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                ? $"Mục đích: {model.MucDichLuuTru}{(model.MucDichLuuTru == "Khác" ? $" ({model.MucDichKhac})" : "")}"
                : $"Mục đích: {model.MucDichLuuTru}{(model.MucDichLuuTru == "Khác" ? $" ({model.MucDichKhac})" : "")}. {model.GhiChu.Trim()}"
        };

        _context.LichSuCuTrus.Add(stay);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Khai báo lưu trú đã được gửi thành công.";
        return RedirectToAction(nameof(DanhSachLuuTru));
    }

    // GET: /LuuTru/SearchForeignerByPassport
    [HttpGet("/LuuTru/SearchForeignerByPassport")]
    public async Task<IActionResult> SearchForeignerByPassport(string passportNo)
    {
        if (string.IsNullOrWhiteSpace(passportNo) || passportNo.Length < 4)
        {
            return Json(new { found = false });
        }

        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.SoHoChieu == passportNo.Trim());

        if (foreigner != null)
        {
            return Json(new
            {
                found = true,
                hoTen = foreigner.HoTen,
                gioiTinh = foreigner.GioiTinh,
                ngaySinh = foreigner.NgaySinh.ToString("yyyy-MM-dd"),
                quocTich = foreigner.QuocTich,
                soHoChieu = foreigner.SoHoChieu,
                loaiHoChieu = "Phổ thông",
                ngayHetHanHoChieu = foreigner.NgayHetHanHoChieu.ToString("yyyy-MM-dd")
            });
        }

        return Json(new { found = false });
    }

    // GET: /accommodation/stays
    [HttpGet("stays")]
    public async Task<IActionResult> DanhSachLuuTru(string? search, int? page)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.LichSuCuTrus
            .Include(l => l.NguoiNuocNgoai)
            .Where(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru && 
                       (l.TrangThai == TrangThaiLuuTru.DangO || l.TrangThai == TrangThaiLuuTru.QuaHan));

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(l => l.NguoiNuocNgoai.HoTen.Contains(search) || 
                                     l.NguoiNuocNgoai.SoHoChieu.Contains(search) || 
                                     (l.Phong != null && l.Phong.Contains(search)));
        }

        ViewBag.Search = search;
        var pagedList = query.OrderByDescending(l => l.NgayBatDau).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // GET: /accommodation/stay-history
    [HttpGet("stay-history")]
    public async Task<IActionResult> LichSuCuTru(string? search, int? page)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.LichSuCuTrus
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

    // POST: /accommodation/update-status
    [HttpPost("update-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(string id, string actionType)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        var stay = await _context.LichSuCuTrus
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

    // GET: /accommodation/facility-info
    [HttpGet("facility-info")]
    public async Task<IActionResult> ThongTinCoSo()
    {
        var facility = await _context.CoSoLuuTrus
            .Include(c => c.ChuCoSoLuuTru).ThenInclude(c => c.TaiKhoan)
            .Include(c => c.PhuongXa)
            .FirstOrDefaultAsync(c => c.ChuCoSoLuuTru.MaTaiKhoan == GetCurrentAccountId());

        if (facility == null) return NotFound();

        return View(facility);
    }

    // GET: /accommodation/update-facility
    [HttpGet("update-facility")]
    public async Task<IActionResult> CapNhatCoSo()
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        var model = new CoSoViewModel
        {
            MaPhuongXa = facility.MaPhuongXa,
            TenCoSo = facility.TenCoSo,
            DiaChi = facility.DiaChi,
            SoDienThoai = facility.SoDienThoai,
            Email = facility.Email
        };

        await LoadWardLookupsAsync(facility.MaPhuongXa);
        return View(model);
    }

    // POST: /accommodation/update-facility
    [HttpPost("update-facility")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatCoSo(CoSoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadWardLookupsAsync(model.MaPhuongXa);
            return View(model);
        }

        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        facility.MaPhuongXa = model.MaPhuongXa;
        facility.TenCoSo = model.TenCoSo;
        facility.DiaChi = model.DiaChi;
        facility.SoDienThoai = model.SoDienThoai;
        facility.Email = model.Email;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin cơ sở lưu trú thành công!";
        return RedirectToAction(nameof(ThongTinCoSo));
    }

    private async Task LoadWardLookupsAsync(int? selectedWardId = null)
    {
        var wards = await _context.PhuongXas
            .OrderBy(p => p.TenPhuongXa)
            .Select(p => new
            {
                p.MaPhuongXa,
                DisplayName = p.TenPhuongXa
            })
            .ToListAsync();

        ViewBag.PhuongXas = new SelectList(wards, "MaPhuongXa", "DisplayName", selectedWardId);
    }
}
