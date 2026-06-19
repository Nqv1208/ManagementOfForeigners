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
        var totalOverdue = stays.Count(s => s.TrangThai == TrangThaiLuuTru.QuaHan);
        var totalLeavingToday = stays.Count(s => s.TrangThai == TrangThaiLuuTru.DangO && s.NgayKetThuc.HasValue && s.NgayKetThuc.Value.Date == DateTime.Today);

        var totalPending = await _context.HoSoKhaiBaoTamTrus
            .CountAsync(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru && h.TrangThai == TrangThaiKhaiBao.ChoDuyet);

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

        var recentDeclarations = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Where(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru)
            .OrderByDescending(h => h.NgayKhaiBao)
            .Take(5)
            .ToListAsync();

        var viewModel = new TongQuanViewModel
        {
            CoSoInfo = facility,
            TotalStaying = totalStaying,
            TotalPendingDeclarations = totalPending,
            TotalLeavingToday = totalLeavingToday,
            TotalOverdue = totalOverdue,
            ActiveStays = activeStaysList,
            RecentCheckins = recentCheckins,
            RecentDeclarations = recentDeclarations
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
            // Tạo TaiKhoan cho người nước ngoài
            var newAccount = new TaiKhoan
            {
                MaTaiKhoan = IdGenerator.NewMaTaiKhoan(_context),
                TenDangNhap = model.SoHoChieu.Trim().ToLower(),
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                MaVaiTro = 1, // Vai trò người nước ngoài là 1
                Email = string.IsNullOrWhiteSpace(model.EmailKhach) ? $"{model.SoHoChieu.Trim().ToLower()}@example.com" : model.EmailKhach.Trim(),
                SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoaiKhach) ? "0000000000" : model.SoDienThoaiKhach.Trim(),
                TrangThai = "Hoạt động",
                NgayTao = DateTime.Now
            };
            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            foreigner = new NguoiNuocNgoai
            {
                MaNguoiNuocNgoai = IdGenerator.NewMaNguoiNuocNgoai(_context),
                MaTaiKhoan = newAccount.MaTaiKhoan,
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
            if (string.IsNullOrEmpty(foreigner.MaTaiKhoan))
            {
                // Tạo TaiKhoan cho người nước ngoài đã có thông tin cá nhân nhưng chưa có tài khoản
                var newAccount = new TaiKhoan
                {
                    MaTaiKhoan = IdGenerator.NewMaTaiKhoan(_context),
                    TenDangNhap = foreigner.SoHoChieu.Trim().ToLower(),
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                    MaVaiTro = 1,
                    Email = string.IsNullOrWhiteSpace(model.EmailKhach) ? $"{foreigner.SoHoChieu.Trim().ToLower()}@example.com" : model.EmailKhach.Trim(),
                    SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoaiKhach) ? "0000000000" : model.SoDienThoaiKhach.Trim(),
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now
                };
                _context.TaiKhoans.Add(newAccount);
                await _context.SaveChangesAsync();

                foreigner.MaTaiKhoan = newAccount.MaTaiKhoan;
            }

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

        // Tạo bản ghi HoSoKhaiBaoTamTru
        var mucDich = model.MucDichLuuTru == "Khác" ? model.MucDichKhac! : model.MucDichLuuTru;
        var ghiChu = string.IsNullOrWhiteSpace(model.SoPhong) 
            ? model.GhiChu 
            : $"Phòng: {model.SoPhong}. {(string.IsNullOrWhiteSpace(model.GhiChu) ? "" : model.GhiChu)}";

        var declaration = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = IdGenerator.NewMaHSKhaiBao(_context),
            MaCoSoLuuTru = facility.MaCoSoLuuTru,
            MaTaiKhoan = foreigner.MaTaiKhoan!,
            NgayKhaiBao = DateTime.Now,
            NgayBatDau = model.NgayBatDau,
            NgayKetThuc = model.NgayKetThucDuKien ?? model.NgayBatDau.AddDays(30),
            MucDichLuuTru = mucDich,
            DiaChiLuuTru = $"{facility.TenCoSo} - {facility.DiaChi}",
            TrangThai = TrangThaiKhaiBao.ChoDuyet,
            GhiChu = ghiChu
        };

        _context.HoSoKhaiBaoTamTrus.Add(declaration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gửi hồ sơ khai báo lưu trú thành công! Đang chờ Công an Phường/Xã phê duyệt.";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // GET: /accommodation/declarations
    [HttpGet("declarations")]
    public async Task<IActionResult> DanhSachKhaiBao(string? search, string? status, int? page)
    {
        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Where(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(h => h.TaiKhoan.NguoiNuocNgoai != null && 
                                     (h.TaiKhoan.NguoiNuocNgoai.HoTen.Contains(search) || 
                                      h.TaiKhoan.NguoiNuocNgoai.SoHoChieu.Contains(search)) ||
                                     h.MaHSKhaiBao.Contains(search));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(h => h.TrangThai == status);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;
        
        var pagedList = query.OrderByDescending(h => h.NgayKhaiBao).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // POST: /accommodation/declarations/cancel
    [HttpPost("declarations/cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyKhaiBao(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var facility = await GetCurrentFacility();
        if (facility == null) return NotFound();

        var declaration = await _context.HoSoKhaiBaoTamTrus
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id && h.MaCoSoLuuTru == facility.MaCoSoLuuTru);

        if (declaration == null) return NotFound();

        if (declaration.TrangThai != TrangThaiKhaiBao.ChoDuyet)
        {
            TempData["ErrorMessage"] = "Chỉ có thể hủy hồ sơ đang chờ duyệt.";
            return RedirectToAction(nameof(DanhSachKhaiBao));
        }

        _context.HoSoKhaiBaoTamTrus.Remove(declaration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã hủy hồ sơ khai báo lưu trú thành công.";
        return RedirectToAction(nameof(DanhSachKhaiBao));
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
    public async Task<IActionResult> LichSuCuTru(string? search, string? status, DateTime? fromDate, DateTime? toDate, int? page)
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
                                     l.NguoiNuocNgoai.SoHoChieu.Contains(search) ||
                                     (l.Phong != null && l.Phong.Contains(search)));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(l => l.TrangThai == status);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.NgayBatDau >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(l => l.NgayBatDau <= toDate.Value);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

        var pagedList = query.OrderByDescending(l => l.NgayBatDau).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // POST: /accommodation/update-status
    [HttpPost("update-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(string id, string actionType, DateTime? checkoutDate, string? note)
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
            stay.NgayKetThuc = checkoutDate ?? DateTime.Now;
            if (!string.IsNullOrWhiteSpace(note))
            {
                stay.GhiChu = string.IsNullOrWhiteSpace(stay.GhiChu)
                    ? $"[Check-out: {note.Trim()}]"
                    : $"{stay.GhiChu} | [Check-out: {note.Trim()}]";
            }
            TempData["SuccessMessage"] = "Đã xác nhận khách trả phòng (Check-out) thành công.";
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

        // Query activity metrics
        ViewBag.ActiveGuestsCount = await _context.LichSuCuTrus
            .CountAsync(l => l.MaCoSoLuuTru == facility.MaCoSoLuuTru && l.TrangThai == TrangThaiLuuTru.DangO);
        ViewBag.TotalDeclarationsCount = await _context.HoSoKhaiBaoTamTrus
            .CountAsync(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru);
        ViewBag.PendingDeclarationsCount = await _context.HoSoKhaiBaoTamTrus
            .CountAsync(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru && h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
        ViewBag.RejectedDeclarationsCount = await _context.HoSoKhaiBaoTamTrus
            .CountAsync(h => h.MaCoSoLuuTru == facility.MaCoSoLuuTru && h.TrangThai == TrangThaiKhaiBao.TuChoi);

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
