using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels;
using ManagementOfForeigners.Helpers;

namespace ManagementOfForeigners.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToDashboard(User.FindFirst(ClaimTypes.Role)?.Value);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = await _context.TaiKhoans
            .Include(t => t.VaiTro)
            .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

        if (taiKhoan == null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (!BCrypt.Net.BCrypt.Verify(model.MatKhau, taiKhoan.MatKhauHash))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (taiKhoan.TrangThai == TrangThaiTaiKhoan.Khoa)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khoá. Vui lòng liên hệ quản trị viên.");
            return View(model);
        }

        // Tạo Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
            new Claim(ClaimTypes.Name, taiKhoan.TenDangNhap),
            new Claim(ClaimTypes.Role, taiKhoan.VaiTro.TenVaiTro)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.GhiNho,
            ExpiresUtc = model.GhiNho ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        // Cập nhật lần đăng nhập cuối
        taiKhoan.LanDangNhapCuoi = DateTime.Now;
        _context.TaiKhoans.Update(taiKhoan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Người dùng {TenDangNhap} đã đăng nhập thành công.", taiKhoan.TenDangNhap);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToDashboard(taiKhoan.VaiTro.TenVaiTro);
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToDashboard(User.FindFirst(ClaimTypes.Role)?.Value);
        }

        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Validate vai trò chỉ được chọn NguoiNuocNgoai hoặc ChuLuuTru
        if (model.VaiTro != VaiTroConst.NguoiNuocNgoai && model.VaiTro != VaiTroConst.ChuLuuTru)
        {
            ModelState.AddModelError("VaiTro", "Loại tài khoản không hợp lệ.");
        }

        // Validate thông tin người nước ngoài
        if (model.VaiTro == VaiTroConst.NguoiNuocNgoai)
        {
            if (string.IsNullOrWhiteSpace(model.HoTen))
                ModelState.AddModelError("HoTen", "Họ tên không được để trống.");
            if (!model.NgaySinh.HasValue)
                ModelState.AddModelError("NgaySinh", "Ngày sinh không được để trống.");
            if (string.IsNullOrWhiteSpace(model.GioiTinh))
                ModelState.AddModelError("GioiTinh", "Giới tính không được để trống.");
            if (string.IsNullOrWhiteSpace(model.QuocTich))
                ModelState.AddModelError("QuocTich", "Quốc tịch không được để trống.");
            if (string.IsNullOrWhiteSpace(model.SoHoChieu))
                ModelState.AddModelError("SoHoChieu", "Số hộ chiếu không được để trống.");
            if (!model.NgayCapHoChieu.HasValue)
                ModelState.AddModelError("NgayCapHoChieu", "Ngày cấp hộ chiếu không được để trống.");
            if (!model.NgayHetHanHoChieu.HasValue)
                ModelState.AddModelError("NgayHetHanHoChieu", "Ngày hết hạn hộ chiếu không được để trống.");
            if (string.IsNullOrWhiteSpace(model.LoaiVisa))
                ModelState.AddModelError("LoaiVisa", "Loại visa không được để trống.");
            if (!model.NgayHetHanVisa.HasValue)
                ModelState.AddModelError("NgayHetHanVisa", "Ngày hết hạn visa không được để trống.");

            // Validate ngày hết hạn hộ chiếu phải sau ngày cấp
            if (model.NgayCapHoChieu.HasValue && model.NgayHetHanHoChieu.HasValue
                && model.NgayHetHanHoChieu.Value <= model.NgayCapHoChieu.Value)
            {
                ModelState.AddModelError("NgayHetHanHoChieu", "Ngày hết hạn hộ chiếu phải sau ngày cấp hộ chiếu.");
            }
        }

        // Validate thông tin cơ sở lưu trú
        if (model.VaiTro == VaiTroConst.ChuLuuTru)
        {
            if (string.IsNullOrWhiteSpace(model.TenCoSo))
                ModelState.AddModelError("TenCoSo", "Tên cơ sở lưu trú không được để trống.");
            if (string.IsNullOrWhiteSpace(model.DiaChiCoSo))
                ModelState.AddModelError("DiaChiCoSo", "Địa chỉ cơ sở không được để trống.");
            if (string.IsNullOrWhiteSpace(model.SoDienThoaiCoSo))
                ModelState.AddModelError("SoDienThoaiCoSo", "Số điện thoại cơ sở không được để trống.");
            if (string.IsNullOrWhiteSpace(model.EmailCoSo))
                ModelState.AddModelError("EmailCoSo", "Email cơ sở không được để trống.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Kiểm tra tên đăng nhập đã tồn tại
        var existingUser = await _context.TaiKhoans
            .AnyAsync(t => t.TenDangNhap == model.TenDangNhap);
        if (existingUser)
        {
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
            return View(model);
        }

        // Kiểm tra email đã tồn tại
        var existingEmail = await _context.TaiKhoans
            .AnyAsync(t => t.Email == model.Email);
        if (existingEmail)
        {
            ModelState.AddModelError("Email", "Email đã được sử dụng.");
            return View(model);
        }

        // Kiểm tra số hộ chiếu đã tồn tại (nếu là người nước ngoài)
        if (model.VaiTro == VaiTroConst.NguoiNuocNgoai)
        {
            var existingPassport = await _context.NguoiNuocNgoais
                .AnyAsync(n => n.SoHoChieu == model.SoHoChieu);
            if (existingPassport)
            {
                ModelState.AddModelError("SoHoChieu", "Số hộ chiếu đã được đăng ký.");
                return View(model);
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Tạo tài khoản
            var maTaiKhoan = IdGenerator.NewMaTaiKhoan(_context);
            var taiKhoan = new TaiKhoan
            {
                MaTaiKhoan = maTaiKhoan,
                TenDangNhap = model.TenDangNhap,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                MaVaiTro = model.VaiTro == VaiTroConst.NguoiNuocNgoai ? 1 : 2,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                TrangThai = TrangThaiTaiKhoan.HoatDong,
                NgayTao = DateTime.Now
            };

            _context.TaiKhoans.Add(taiKhoan);

            // Tạo NguoiNuocNgoai nếu vai trò là NguoiNuocNgoai
            if (model.VaiTro == VaiTroConst.NguoiNuocNgoai)
            {
                var maNguoiNuocNgoai = IdGenerator.NewMaNguoiNuocNgoai(_context);
                var nguoiNuocNgoai = new NguoiNuocNgoai
                {
                    MaNguoiNuocNgoai = maNguoiNuocNgoai,
                    MaTaiKhoan = maTaiKhoan,
                    HoTen = model.HoTen!,
                    NgaySinh = model.NgaySinh!.Value,
                    GioiTinh = model.GioiTinh!,
                    QuocTich = model.QuocTich!,
                    SoHoChieu = model.SoHoChieu!,
                    NgayCapHoChieu = model.NgayCapHoChieu!.Value,
                    NgayHetHanHoChieu = model.NgayHetHanHoChieu!.Value,
                    LoaiVisa = model.LoaiVisa!,
                    NgayHetHanVisa = model.NgayHetHanVisa!.Value
                };

                _context.NguoiNuocNgoais.Add(nguoiNuocNgoai);
            }

            // Tạo CoSoLuuTru & ChuCoSoLuuTru nếu vai trò là ChuLuuTru
            if (model.VaiTro == VaiTroConst.ChuLuuTru)
            {
                // 1. Tạo ChuCoSoLuuTru
                var maChuCoSo = IdGenerator.NewMaChuCoSo(_context);
                var chuCoSo = new ChuCoSoLuuTru
                {
                    MaChuCoSo = maChuCoSo,
                    MaTaiKhoan = maTaiKhoan,
                    HoTen = "Chủ cơ sở " + model.TenCoSo, // Tên tạm thời
                    NgaySinh = new DateTime(1980, 1, 1),
                    GioiTinh = "Nam",
                    SoCCCD = "000000000000",
                    NgayCapCCCD = new DateTime(2020, 1, 1),
                    NoiCapCCCD = "Cục Cảnh sát cư trú",
                    DiaChiThuongTru = model.DiaChiCoSo
                };
                _context.ChuCoSoLuuTrus.Add(chuCoSo);

                // 2. Tạo CoSoLuuTru
                var maCoSoLuuTru = IdGenerator.NewMaCoSoLuuTru(_context);
                var coSoLuuTru = new CoSoLuuTru
                {
                    MaCoSoLuuTru = maCoSoLuuTru,
                    MaTaiKhoan = maTaiKhoan,
                    TenCoSo = model.TenCoSo!,
                    DiaChi = model.DiaChiCoSo!,
                    SoDienThoai = model.SoDienThoaiCoSo!,
                    Email = model.EmailCoSo!,
                    TrangThai = TrangThaiCoSo.DangHoatDong
                };

                _context.CoSoLuuTrus.Add(coSoLuuTru);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Tài khoản mới đã được tạo: {TenDangNhap}, vai trò: {VaiTro}.", model.TenDangNhap, model.VaiTro);

            // Tự động đăng nhập sau khi đăng ký
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
                new Claim(ClaimTypes.Name, taiKhoan.TenDangNhap),
                new Claim(ClaimTypes.Role, model.VaiTro)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false });

            // Cập nhật lần đăng nhập cuối
            taiKhoan.LanDangNhapCuoi = DateTime.Now;
            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Chào mừng bạn đến với hệ thống.";
            return RedirectToDashboard(model.VaiTro);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi khi đăng ký tài khoản mới.");
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.");
            return View(model);
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Người dùng {TenDangNhap} đã đăng xuất.", userName);
        TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Chuyển hướng đến trang Dashboard tương ứng theo vai trò
    /// </summary>
    private IActionResult RedirectToDashboard(string? vaiTro)
    {
        return vaiTro switch
        {
            VaiTroConst.NguoiNuocNgoai => RedirectToAction("Dashboard", "Foreigner"),
            VaiTroConst.ChuLuuTru => RedirectToAction("Dashboard", "Accommodation"),
            VaiTroConst.CanBoXNC => RedirectToAction("Dashboard", "Officer"),
            VaiTroConst.CongAn => RedirectToAction("Dashboard", "Police"),
            VaiTroConst.Admin => RedirectToAction("Index", "Home"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
