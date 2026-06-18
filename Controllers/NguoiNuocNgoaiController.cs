using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.NguoiNuocNgoai;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.NguoiNuocNgoai)]
[Route("foreigner")]
public class NguoiNuocNgoaiController : Controller
{
    private readonly ApplicationDbContext _context;

    public NguoiNuocNgoaiController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    // GET: /foreigner/dashboard
    [HttpGet("dashboard")]
    public IActionResult TongQuan()
    {
        return RedirectToAction(nameof(ThongTinCaNhan));
    }

    // GET: /foreigner/profile
    [HttpGet("profile")]
    public async Task<IActionResult> ThongTinCaNhan()
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner?.TaiKhoan == null) return NotFound();

        return View(foreigner);
    }

    // GET: /foreigner/update-profile
    [HttpGet("update-profile")]
    public async Task<IActionResult> CapNhatThongTin()
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner?.TaiKhoan == null) return NotFound();

        var account = foreigner.TaiKhoan;

        var model = new CapNhatThongTinViewModel
        {
            HoTen = foreigner.HoTen,
            NgaySinh = foreigner.NgaySinh,
            GioiTinh = foreigner.GioiTinh,
            QuocTich = foreigner.QuocTich,
            SoHoChieu = foreigner.SoHoChieu,
            NgayCapHoChieu = foreigner.NgayCapHoChieu,
            NgayHetHanHoChieu = foreigner.NgayHetHanHoChieu,
            Email = account.Email ?? string.Empty,
            SoDienThoai = account.SoDienThoai ?? string.Empty
        };

        return View(model);
    }

    // POST: /foreigner/update-profile
    [HttpPost("update-profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTin(CapNhatThongTinViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner?.TaiKhoan == null) return NotFound();

        var account = foreigner.TaiKhoan;

        // Kiểm tra xem số hộ chiếu mới có bị trùng với người khác không
        if (foreigner.SoHoChieu != model.SoHoChieu)
        {
            var exists = await _context.NguoiNuocNgoais
                .AnyAsync(n => n.SoHoChieu == model.SoHoChieu && n.MaTaiKhoan != accountId);
            if (exists)
            {
                ModelState.AddModelError("SoHoChieu", "Số hộ chiếu này đã tồn tại trên hệ thống");
                return View(model);
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Ghi nhận lịch sử thay đổi (Chỉ lưu vết các trường có thay đổi)
            var now = DateTime.Now;

            void LogChange(string fieldName, string? oldValue, string newValue)
            {
                if (oldValue != newValue)
                {
                    var lsc = new LichSuCapNhatThongTinCaNhan
                    {
                        MaLSCapNhat = IdGenerator.NewMaLSCapNhat(_context),
                        MaTaiKhoan = accountId,
                        TruongCapNhat = fieldName,
                        GiaTriCu = oldValue,
                        GiaTriMoi = newValue,
                        NgayCapNhat = now,
                        LyDoCapNhat = model.LyDoCapNhat,
                        TrangThai = "Đã cập nhật"
                    };
                    _context.LichSuCapNhatThongTinCaNhans.Add(lsc);
                }
            }

            LogChange("Họ tên", foreigner.HoTen, model.HoTen);
            LogChange("Ngày sinh", foreigner.NgaySinh.ToString("yyyy-MM-dd"), model.NgaySinh.ToString("yyyy-MM-dd"));
            LogChange("Giới tính", foreigner.GioiTinh, model.GioiTinh);
            LogChange("Quốc tịch", foreigner.QuocTich, model.QuocTich);
            LogChange("Số hộ chiếu", foreigner.SoHoChieu, model.SoHoChieu);
            LogChange("Ngày cấp HC", foreigner.NgayCapHoChieu.ToString("yyyy-MM-dd"), model.NgayCapHoChieu.ToString("yyyy-MM-dd"));
            LogChange("Ngày hết hạn HC", foreigner.NgayHetHanHoChieu.ToString("yyyy-MM-dd"), model.NgayHetHanHoChieu.ToString("yyyy-MM-dd"));
            LogChange("Email", account.Email, model.Email);
            LogChange("Số điện thoại", account.SoDienThoai, model.SoDienThoai);

            // Cập nhật giá trị
            foreigner.HoTen = model.HoTen;
            foreigner.NgaySinh = model.NgaySinh;
            foreigner.GioiTinh = model.GioiTinh;
            foreigner.QuocTich = model.QuocTich;
            foreigner.SoHoChieu = model.SoHoChieu;
            foreigner.NgayCapHoChieu = model.NgayCapHoChieu;
            foreigner.NgayHetHanHoChieu = model.NgayHetHanHoChieu;
            
            account.Email = model.Email;
            account.SoDienThoai = model.SoDienThoai;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân và lưu vết lịch sử thành công!";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình lưu dữ liệu. Vui lòng thử lại.";
            return View(model);
        }
    }

    // GET: /foreigner/search-facilities
    // GET: /KhaiBaoTamTru/SearchCoSoLuuTru
    [HttpGet("search-facilities")]
    [HttpGet("/KhaiBaoTamTru/SearchCoSoLuuTru")]
    public async Task<IActionResult> SearchCoSoLuuTru(string? keyword, int? phuongXaId)
    {
        var searchKeyword = keyword?.Trim();
        if (string.IsNullOrWhiteSpace(searchKeyword) || searchKeyword.Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var query = _context.CoSoLuuTrus
            .Include(c => c.PhuongXa)
            .Where(c => c.TrangThai == TrangThaiCoSo.DangHoatDong);

        if (phuongXaId.HasValue)
        {
            query = query.Where(c => c.MaPhuongXa == phuongXaId.Value);
        }

        var normalizedKeyword = searchKeyword.ToLower();
        query = query.Where(c =>
            c.TenCoSo.ToLower().Contains(normalizedKeyword) ||
            c.DiaChi.ToLower().Contains(normalizedKeyword) ||
            c.MaCoSoLuuTru.ToLower().Contains(normalizedKeyword)
        );

        var results = await query
            .OrderBy(c => c.TenCoSo)
            .Take(10)
            .Select(c => new
            {
                id = c.MaCoSoLuuTru,
                tenCoSo = c.TenCoSo,
                diaChi = c.DiaChi,
                phuongXa = c.PhuongXa.TenPhuongXa,
                soDienThoai = c.SoDienThoai,
                isActive = c.TrangThai == TrangThaiCoSo.DangHoatDong
            })
            .ToListAsync();

        return Json(results);
    }

    // GET: /foreigner/declare-residence
    [HttpGet("declare-residence")]
    public async Task<IActionResult> KhaiBaoTamTru()
    {
        var accountId = GetCurrentAccountId();
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(t => t.MaTaiKhoan == accountId);

        if (account == null) return Challenge();

        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ người nước ngoài. Vui lòng liên hệ quản trị viên.";
            return RedirectToAction("ThongTinCaNhan");
        }

        var model = new KhaiBaoTamTruCreateViewModel
        {
            HoTen = foreigner.HoTen,
            GioiTinh = foreigner.GioiTinh,
            NgaySinh = foreigner.NgaySinh,
            QuocTich = foreigner.QuocTich,
            SoHoChieu = foreigner.SoHoChieu,
            NgayHetHanHoChieu = foreigner.NgayHetHanHoChieu,
            LoaiVisa = foreigner.LoaiVisa,
            NgayHetHanVisa = foreigner.NgayHetHanVisa,
            Email = account.Email ?? string.Empty,
            SoDienThoai = account.SoDienThoai ?? string.Empty,
            NgayBatDau = DateTime.Today,
            NgayKetThuc = DateTime.Today.AddMonths(1)
        };

        var wards = await _context.PhuongXas
            .OrderBy(p => p.TenPhuongXa)
            .Select(p => new SelectListItem
            {
                Value = p.MaPhuongXa.ToString(),
                Text = p.TenPhuongXa
            })
            .ToListAsync();

        model.PhuongXaOptions = wards;

        return View(model);
    }

    // POST: /foreigner/declare-residence
    [HttpPost("declare-residence")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhaiBaoTamTru(KhaiBaoTamTruCreateViewModel model)
    {
        var accountId = GetCurrentAccountId();
        var account = await _context.TaiKhoans
            .FirstOrDefaultAsync(t => t.MaTaiKhoan == accountId);

        if (account == null) return Challenge();

        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null)
        {
            TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ người nước ngoài.";
            return RedirectToAction("ThongTinCaNhan");
        }

        if (model.NgayKetThuc <= model.NgayBatDau)
        {
            ModelState.AddModelError(nameof(model.NgayKetThuc), "Ngày kết thúc phải lớn hơn ngày bắt đầu");
        }

        if (model.NgayBatDau.Date < DateTime.Today.AddDays(-7))
        {
            ModelState.AddModelError(nameof(model.NgayBatDau), "Ngày bắt đầu không được nhỏ hơn quá khứ quá 7 ngày");
        }

        if (!model.PhuongXaId.HasValue)
        {
            ModelState.AddModelError(nameof(model.PhuongXaId), "Vui lòng chọn Phường/Xã lưu trú");
        }
        else
        {
            var wardExists = await _context.PhuongXas.AnyAsync(p => p.MaPhuongXa == model.PhuongXaId.Value);
            if (!wardExists)
            {
                ModelState.AddModelError(nameof(model.PhuongXaId), "Phường/Xã lưu trú không tồn tại");
            }
        }

        CoSoLuuTru? facility = null;
        if (string.IsNullOrWhiteSpace(model.CoSoLuuTruId))
        {
            ModelState.AddModelError(nameof(model.CoSoLuuTruId), "Vui lòng chọn cơ sở lưu trú từ danh sách gợi ý");
        }
        else
        {
            facility = await _context.CoSoLuuTrus
                .FirstOrDefaultAsync(c => c.MaCoSoLuuTru == model.CoSoLuuTruId && c.TrangThai == TrangThaiCoSo.DangHoatDong);
            
            if (facility == null)
            {
                ModelState.AddModelError(nameof(model.CoSoLuuTruId), "Cơ sở lưu trú không tồn tại hoặc đã ngừng hoạt động");
            }
            else if (model.PhuongXaId.HasValue && facility.MaPhuongXa != model.PhuongXaId.Value)
            {
                ModelState.AddModelError(nameof(model.CoSoLuuTruId), "Cơ sở lưu trú đã chọn không thuộc Phường/Xã lưu trú đã chọn");
            }
        }

        if (model.MucDichLuuTru == "Khác" && string.IsNullOrWhiteSpace(model.MucDichKhac))
        {
            ModelState.AddModelError(nameof(model.MucDichKhac), "Vui lòng nhập mục đích cụ thể");
        }

        if (!model.CamKetThongTin)
        {
            ModelState.AddModelError(nameof(model.CamKetThongTin), "Vui lòng xác nhận cam kết trước khi gửi");
        }

        if (!ModelState.IsValid)
        {
            model.HoTen = foreigner.HoTen;
            model.GioiTinh = foreigner.GioiTinh;
            model.NgaySinh = foreigner.NgaySinh;
            model.QuocTich = foreigner.QuocTich;
            model.SoHoChieu = foreigner.SoHoChieu;
            model.NgayHetHanHoChieu = foreigner.NgayHetHanHoChieu;
            model.LoaiVisa = foreigner.LoaiVisa;
            model.NgayHetHanVisa = foreigner.NgayHetHanVisa;
            model.Email = account.Email ?? string.Empty;
            model.SoDienThoai = account.SoDienThoai ?? string.Empty;

            var wards = await _context.PhuongXas
                .OrderBy(p => p.TenPhuongXa)
                .Select(p => new SelectListItem
                {
                    Value = p.MaPhuongXa.ToString(),
                    Text = p.TenPhuongXa
                })
                .ToListAsync();

            model.PhuongXaOptions = wards;
            return View(model);
        }

        var mucDich = model.MucDichLuuTru == "Khác" ? model.MucDichKhac! : model.MucDichLuuTru;
        var ghiChu = string.IsNullOrWhiteSpace(model.SoPhong) 
            ? model.GhiChu 
            : $"Phòng/Căn hộ: {model.SoPhong}. {(string.IsNullOrWhiteSpace(model.GhiChu) ? "" : model.GhiChu)}";

        var declaration = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = IdGenerator.NewMaHSKhaiBao(_context),
            MaTaiKhoan = accountId,
            NgayKhaiBao = DateTime.Now,
            NgayBatDau = model.NgayBatDau,
            NgayKetThuc = model.NgayKetThuc,
            MucDichLuuTru = mucDich,
            DiaChiLuuTru = model.DiaChiLuuTruCuThe,
            MaCoSoLuuTru = model.CoSoLuuTruId,
            TrangThai = TrangThaiKhaiBao.ChoDuyet,
            GhiChu = ghiChu
        };

        _context.HoSoKhaiBaoTamTrus.Add(declaration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gửi yêu cầu khai báo tạm trú thành công! Chờ Công an Phường/Xã phê duyệt.";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // GET: /foreigner/declarations
    [HttpGet("declarations")]
    public IActionResult DanhSachKhaiBao(int? page)
    {
        var accountId = GetCurrentAccountId();
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var declarations = _context.HoSoKhaiBaoTamTrus
            .Include(h => h.CoSoLuuTru)
            .Where(h => h.MaTaiKhoan == accountId)
            .OrderByDescending(h => h.NgayKhaiBao)
            .ToPagedList(pageNumber, pageSize);

        return View(declarations);
    }

    // GET: /foreigner/declaration/{id}
    [HttpGet("declaration/{id}")]
    public async Task<IActionResult> ChiTietKhaiBao(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var accountId = GetCurrentAccountId();
        var declaration = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.CoSoLuuTru)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id && h.MaTaiKhoan == accountId);

        if (declaration == null) return NotFound();

        return View(declaration);
    }

    // GET: /foreigner/stay-history
    [HttpGet("stay-history")]
    public async Task<IActionResult> LichSuCuTru(int? page)
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

        int pageSize = 10;
        int pageNumber = page ?? 1;

        var history = _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai)
            .OrderByDescending(l => l.NgayBatDau)
            .ToPagedList(pageNumber, pageSize);

        ViewBag.CuTruModel = new CuTruViewModel();

        return View(history);
    }

    // POST: /foreigner/update-residence
    [HttpPost("update-residence")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatNoiCuTru(CuTruViewModel model)
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

        CoSoLuuTru? facility = null;
        if (string.IsNullOrWhiteSpace(model.MaCoSoLuuTru))
        {
            ModelState.AddModelError(nameof(model.MaCoSoLuuTru), "Vui lòng chọn cơ sở lưu trú từ danh sách gợi ý.");
        }
        else
        {
            facility = await _context.CoSoLuuTrus
                .FirstOrDefaultAsync(c => c.MaCoSoLuuTru == model.MaCoSoLuuTru);

            if (facility == null)
            {
                ModelState.AddModelError(nameof(model.MaCoSoLuuTru), "Cơ sở lưu trú không tồn tại.");
            }
            else if (facility.TrangThai != TrangThaiCoSo.DangHoatDong)
            {
                ModelState.AddModelError(nameof(model.MaCoSoLuuTru), "Cơ sở lưu trú đã chọn không còn hoạt động.");
            }
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Dữ liệu khai báo nơi cư trú không hợp lệ.";
            return RedirectToAction(nameof(LichSuCuTru));
        }

        // Cập nhật trạng thái của nơi cư trú cũ thành "Đã rời" (nếu đang ở)
        var currentResidence = await _context.LichSuCuTrus
            .Where(l => l.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai && l.TrangThai == TrangThaiLuuTru.DangO)
            .ToListAsync();

        foreach (var res in currentResidence)
        {
            res.TrangThai = TrangThaiLuuTru.DaRoi;
            res.NgayKetThuc = model.NgayBatDau; // Ngày rời đi bằng ngày bắt đầu của nơi ở mới
        }

        // Tạo bản ghi cư trú mới
        var newResidence = new LichSuCuTru
        {
            MaLSLuuTru = IdGenerator.NewMaLichSuCuTru(_context),
            MaNguoiNuocNgoai = foreigner.MaNguoiNuocNgoai,
            MaCoSoLuuTru = facility!.MaCoSoLuuTru,
            NgayBatDau = model.NgayBatDau,
            Phong = model.Phong,
            TrangThai = TrangThaiLuuTru.DangO,
            GhiChu = model.GhiChu
        };

        _context.LichSuCuTrus.Add(newResidence);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Khai báo cập nhật nơi cư trú mới thành công!";
        return RedirectToAction(nameof(LichSuCuTru));
    }

    // GET: /foreigner/settings
    [HttpGet("settings")]
    public async Task<IActionResult> CaiDat()
    {
        var accountId = GetCurrentAccountId();
        var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == accountId);
        if (account == null) return NotFound();
        return View(account);
    }

    // GET: /foreigner/alerts
    [HttpGet("alerts")]
    public async Task<IActionResult> CanhBao()
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais.FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);
        if (foreigner == null) return NotFound();
        var warnings = await _context.CanhBaoViPhams
            .Where(c => c.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai)
            .OrderByDescending(c => c.NgayCanhBao)
            .ToListAsync();
        return View(warnings);
    }

    // GET: /foreigner/guides
    [HttpGet("guides")]
    public IActionResult HuongDan()
    {
        return View();
    }
}
