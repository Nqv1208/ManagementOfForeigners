using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Foreigner;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.NguoiNuocNgoai)]
public class ForeignerController : Controller
{
    private readonly ApplicationDbContext _context;

    public ForeignerController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    // GET: Foreigner/Dashboard
    public async Task<IActionResult> Dashboard()
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
            return RedirectToAction("Index", "Home");
        }

        var recentDeclarations = await _context.HoSoKhaiBaoTamTrus
            .Where(h => h.MaTaiKhoan == accountId)
            .OrderByDescending(h => h.NgayKhaiBao)
            .Take(5)
            .ToListAsync();

        var recentResidence = await _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai)
            .OrderByDescending(l => l.NgayBatDau)
            .Take(5)
            .ToListAsync();

        var activeWarnings = await _context.CanhBaoViPhams
            .Where(c => c.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai)
            .OrderByDescending(c => c.NgayCanhBao)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            AccountInfo = account,
            PersonalInfo = foreigner,
            RecentDeclarations = recentDeclarations,
            RecentResidenceHistory = recentResidence,
            ActiveWarnings = activeWarnings
        };

        return View(viewModel);
    }

    // GET: Foreigner/ThongTinCaNhan
    public async Task<IActionResult> ThongTinCaNhan()
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

        return View(foreigner);
    }

    // GET: Foreigner/CapNhatThongTin
    public async Task<IActionResult> CapNhatThongTin()
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

        var model = new CapNhatThongTinViewModel
        {
            HoTen = foreigner.HoTen,
            NgaySinh = foreigner.NgaySinh,
            GioiTinh = foreigner.GioiTinh,
            QuocTich = foreigner.QuocTich,
            SoHoChieu = foreigner.SoHoChieu,
            NgayCapHoChieu = foreigner.NgayCapHoChieu,
            NgayHetHanHoChieu = foreigner.NgayHetHanHoChieu,
            Email = foreigner.TaiKhoan.Email ?? string.Empty,
            SoDienThoai = foreigner.TaiKhoan.SoDienThoai ?? string.Empty
        };

        return View(model);
    }

    // POST: Foreigner/CapNhatThongTin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTin(CapNhatThongTinViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .Include(n => n.TaiKhoan)
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

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
            LogChange("Email", foreigner.TaiKhoan.Email, model.Email);
            LogChange("Số điện thoại", foreigner.TaiKhoan.SoDienThoai, model.SoDienThoai);

            // Cập nhật giá trị
            foreigner.HoTen = model.HoTen;
            foreigner.NgaySinh = model.NgaySinh;
            foreigner.GioiTinh = model.GioiTinh;
            foreigner.QuocTich = model.QuocTich;
            foreigner.SoHoChieu = model.SoHoChieu;
            foreigner.NgayCapHoChieu = model.NgayCapHoChieu;
            foreigner.NgayHetHanHoChieu = model.NgayHetHanHoChieu;
            
            foreigner.TaiKhoan.Email = model.Email;
            foreigner.TaiKhoan.SoDienThoai = model.SoDienThoai;

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

    // GET: Foreigner/KhaiBaoTamTru
    public async Task<IActionResult> KhaiBaoTamTru()
    {
        var facilities = await _context.CoSoLuuTrus
            .Where(c => c.TrangThai == TrangThaiCoSo.DangHoatDong)
            .Select(c => new SelectListItem
            {
                Value = c.MaCoSoLuuTru,
                Text = $"{c.TenCoSo} - {c.DiaChi}"
            }).ToListAsync();

        ViewBag.CoSoLuuTrus = new SelectList(facilities, "Value", "Text");
        return View(new KhaiBaoViewModel());
    }

    // POST: Foreigner/KhaiBaoTamTru
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhaiBaoTamTru(KhaiBaoViewModel model)
    {
        if (model.NgayKetThuc <= model.NgayBatDau)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
        }

        if (!ModelState.IsValid)
        {
            var facilities = await _context.CoSoLuuTrus
                .Where(c => c.TrangThai == TrangThaiCoSo.DangHoatDong)
                .Select(c => new SelectListItem
                {
                    Value = c.MaCoSoLuuTru,
                    Text = $"{c.TenCoSo} - {c.DiaChi}"
                }).ToListAsync();
            ViewBag.CoSoLuuTrus = new SelectList(facilities, "Value", "Text");
            return View(model);
        }

        var accountId = GetCurrentAccountId();

        var declaration = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = IdGenerator.NewMaHSKhaiBao(_context),
            MaTaiKhoan = accountId,
            NgayKhaiBao = DateTime.Now,
            NgayBatDau = model.NgayBatDau,
            NgayKetThuc = model.NgayKetThuc,
            MucDichLuuTru = model.MucDichLuuTru,
            DiaChiLuuTru = model.DiaChiLuuTru,
            MaCoSoLuuTru = model.MaCoSoLuuTru,
            TrangThai = TrangThaiKhaiBao.ChoDuyet,
            GhiChu = model.GhiChu
        };

        _context.HoSoKhaiBaoTamTrus.Add(declaration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gửi yêu cầu khai báo tạm trú thành công! Chờ Công an Phường/Xã phê duyệt.";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // GET: Foreigner/DanhSachKhaiBao
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

    // GET: Foreigner/ChiTietKhaiBao/5
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

    // GET: Foreigner/LichSuCuTru
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

        var facilities = await _context.CoSoLuuTrus
            .Where(c => c.TrangThai == TrangThaiCoSo.DangHoatDong)
            .Select(c => new SelectListItem
            {
                Value = c.MaCoSoLuuTru,
                Text = $"{c.TenCoSo} - {c.DiaChi}"
            }).ToListAsync();

        ViewBag.CoSoLuuTrus = new SelectList(facilities, "Value", "Text");
        ViewBag.CuTruModel = new CuTruViewModel();

        return View(history);
    }

    // POST: Foreigner/CapNhatNoiCuTru
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatNoiCuTru(CuTruViewModel model)
    {
        var accountId = GetCurrentAccountId();
        var foreigner = await _context.NguoiNuocNgoais
            .FirstOrDefaultAsync(n => n.MaTaiKhoan == accountId);

        if (foreigner == null) return NotFound();

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
            MaCoSoLuuTru = model.MaCoSoLuuTru,
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
}
