using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels;
using ManagementOfForeigners.Helpers;

namespace ManagementOfForeigners.Controllers;

[Route("account")]
public class TaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaiKhoanController> _logger;

    public TaiKhoanController(ApplicationDbContext context, ILogger<TaiKhoanController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /account/login
    [HttpGet("login")]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToDashboard(User.FindFirst(ClaimTypes.Role)?.Value);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /account/login
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginKey = model.TenDangNhap.Trim();
        model.TenDangNhap = loginKey;

        var taiKhoan = await _context.TaiKhoans
            .Include(t => t.VaiTro)
            .FirstOrDefaultAsync(t => t.TenDangNhap == loginKey || t.Email == loginKey);

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

        if (taiKhoan.TrangThai == TrangThaiTaiKhoan.ChoDuyet)
        {
            return RedirectToAction(nameof(PendingApproval));
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

    // GET: /account/register
    [HttpGet("register")]
    public IActionResult DangKy()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToDashboard(User.FindFirst(ClaimTypes.Role)?.Value);
        }

        var model = new DangKyUnifiedViewModel { AccountType = "Foreigner" };
        LoadRegisterLookups();
        return View(model);
    }

    // POST: /account/register
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(DangKyUnifiedViewModel model)
    {
        // Perform conditional model state validation
        if (model.AccountType == "Foreigner")
        {
            if (string.IsNullOrWhiteSpace(model.HoTen))
                ModelState.AddModelError(nameof(model.HoTen), "Họ tên không được để trống.");
            if (model.NgaySinh == null)
                ModelState.AddModelError(nameof(model.NgaySinh), "Ngày sinh không được để trống.");
            if (string.IsNullOrWhiteSpace(model.GioiTinh))
                ModelState.AddModelError(nameof(model.GioiTinh), "Giới tính không được để trống.");
            if (string.IsNullOrWhiteSpace(model.QuocTich))
                ModelState.AddModelError(nameof(model.QuocTich), "Quốc tịch không được để trống.");
            if (string.IsNullOrWhiteSpace(model.SoHoChieu))
                ModelState.AddModelError(nameof(model.SoHoChieu), "Số hộ chiếu không được để trống.");
        }
        else if (model.AccountType == "LodgingOwner")
        {
            if (string.IsNullOrWhiteSpace(model.HoTenNguoiDaiDien))
                ModelState.AddModelError(nameof(model.HoTenNguoiDaiDien), "Họ tên người đại diện không được để trống.");
            if (string.IsNullOrWhiteSpace(model.SoGiayToNguoiDaiDien))
                ModelState.AddModelError(nameof(model.SoGiayToNguoiDaiDien), "Số giấy tờ người đại diện không được để trống.");
            if (string.IsNullOrWhiteSpace(model.TenCoSoLuuTru))
                ModelState.AddModelError(nameof(model.TenCoSoLuuTru), "Tên cơ sở lưu trú không được để trống.");
            if (string.IsNullOrWhiteSpace(model.SoDienThoaiCoSo))
                ModelState.AddModelError(nameof(model.SoDienThoaiCoSo), "Số điện thoại cơ sở không được để trống.");
            if (model.PhuongXaId == null)
                ModelState.AddModelError(nameof(model.PhuongXaId), "Vui lòng chọn Phường/Xã.");
            if (string.IsNullOrWhiteSpace(model.DiaChiCoSo))
                ModelState.AddModelError(nameof(model.DiaChiCoSo), "Địa chỉ cơ sở không được để trống.");
        }

        if (ModelState.IsValid)
        {
            // Uniqueness checks in database: TenDangNhap, Email
            var existingUser = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap);
            if (existingUser)
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            }

            var existingEmail = await _context.TaiKhoans.AnyAsync(t => t.Email == model.Email);
            if (existingEmail)
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
            }

            if (model.AccountType == "Foreigner")
            {
                if (!string.IsNullOrWhiteSpace(model.SoHoChieu))
                {
                    var existingPassport = await _context.NguoiNuocNgoais.AnyAsync(n => n.SoHoChieu == model.SoHoChieu);
                    if (existingPassport)
                    {
                        ModelState.AddModelError(nameof(model.SoHoChieu), "Số hộ chiếu đã được đăng ký.");
                    }
                }
            }
            else if (model.AccountType == "LodgingOwner")
            {
                if (!string.IsNullOrWhiteSpace(model.SoGiayToNguoiDaiDien))
                {
                    var existingCccd = await _context.ChuCoSoLuuTrus.AnyAsync(c => c.SoCCCD == model.SoGiayToNguoiDaiDien);
                    if (existingCccd)
                    {
                        ModelState.AddModelError(nameof(model.SoGiayToNguoiDaiDien), "Số giấy tờ người đại diện đã được đăng ký.");
                    }
                }

                if (model.PhuongXaId != null)
                {
                    var wardExists = await _context.PhuongXas.AnyAsync(p => p.MaPhuongXa == model.PhuongXaId);
                    if (!wardExists)
                    {
                        ModelState.AddModelError(nameof(model.PhuongXaId), "Phường/Xã được chọn không hợp lệ.");
                    }
                }
            }
        }

        if (!ModelState.IsValid)
        {
            LoadRegisterLookups(model.PhuongXaId);
            return View(model);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var maTaiKhoan = IdGenerator.NewMaTaiKhoan(_context);

            if (model.AccountType == "Foreigner")
            {
                var taiKhoan = new TaiKhoan
                {
                    MaTaiKhoan = maTaiKhoan,
                    TenDangNhap = model.TenDangNhap,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    MaVaiTro = 1, // NguoiNuocNgoai
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    TrangThai = TrangThaiTaiKhoan.HoatDong,
                    NgayTao = DateTime.Now
                };
                _context.TaiKhoans.Add(taiKhoan);

                var maNguoiNuocNgoai = IdGenerator.NewMaNguoiNuocNgoai(_context);
                var nguoiNuocNgoai = new NguoiNuocNgoai
                {
                    MaNguoiNuocNgoai = maNguoiNuocNgoai,
                    MaTaiKhoan = maTaiKhoan,
                    HoTen = model.HoTen ?? string.Empty,
                    NgaySinh = model.NgaySinh ?? DateTime.Today,
                    GioiTinh = model.GioiTinh ?? "Khác",
                    QuocTich = model.QuocTich ?? string.Empty,
                    SoHoChieu = model.SoHoChieu ?? string.Empty,
                    NgayCapHoChieu = DateTime.Today,
                    NgayHetHanHoChieu = model.HanHoChieu ?? DateTime.Today.AddYears(5),
                    LoaiVisa = "Du lịch",
                    NgayHetHanVisa = model.HanHoChieu ?? DateTime.Today.AddYears(1)
                };
                _context.NguoiNuocNgoais.Add(nguoiNuocNgoai);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Đăng ký tài khoản người nước ngoài thành công: {TenDangNhap}", model.TenDangNhap);

                // Tự động đăng nhập
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
                    new Claim(ClaimTypes.Name, taiKhoan.TenDangNhap),
                    new Claim(ClaimTypes.Role, VaiTroConst.NguoiNuocNgoai)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties { IsPersistent = false });

                taiKhoan.LanDangNhapCuoi = DateTime.Now;
                _context.TaiKhoans.Update(taiKhoan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký tài khoản người nước ngoài thành công!";
                return RedirectToDashboard(VaiTroConst.NguoiNuocNgoai);
            }
            else // LodgingOwner
            {
                var taiKhoan = new TaiKhoan
                {
                    MaTaiKhoan = maTaiKhoan,
                    TenDangNhap = model.TenDangNhap,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    MaVaiTro = 2, // ChuLuuTru
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    TrangThai = TrangThaiTaiKhoan.ChoDuyet,
                    NgayTao = DateTime.Now
                };
                _context.TaiKhoans.Add(taiKhoan);

                var maChuCoSo = IdGenerator.NewMaChuCoSo(_context);
                var chuCoSo = new ChuCoSoLuuTru
                {
                    MaChuCoSo = maChuCoSo,
                    MaTaiKhoan = maTaiKhoan,
                    HoTen = model.HoTenNguoiDaiDien ?? string.Empty,
                    NgaySinh = model.NgaySinhNguoiDaiDien ?? DateTime.Today,
                    GioiTinh = model.GioiTinhNguoiDaiDien ?? "Khác",
                    SoCCCD = model.SoGiayToNguoiDaiDien ?? string.Empty,
                    NgayCapCCCD = DateTime.Today,
                    NoiCapCCCD = "Chưa cập nhật",
                    DiaChiThuongTru = model.DiaChiCoSo
                };
                _context.ChuCoSoLuuTrus.Add(chuCoSo);

                var maCoSoLuuTru = IdGenerator.NewMaCoSoLuuTru(_context);
                var coSoLuuTru = new CoSoLuuTru
                {
                    MaCoSoLuuTru = maCoSoLuuTru,
                    MaChuCoSo = maChuCoSo,
                    MaPhuongXa = model.PhuongXaId ?? 0,
                    TenCoSo = model.TenCoSoLuuTru ?? string.Empty,
                    DiaChi = model.DiaChiCoSo ?? string.Empty,
                    SoDienThoai = model.SoDienThoaiCoSo ?? string.Empty,
                    Email = model.EmailCoSo ?? string.Empty,
                    TrangThai = TrangThaiCoSo.ChoDuyet
                };
                _context.CoSoLuuTrus.Add(coSoLuuTru);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Đăng ký tài khoản cơ sở lưu trú thành công (Chờ duyệt): {TenDangNhap}", model.TenDangNhap);

                TempData["SuccessMessage"] = "Đăng ký tài khoản cơ sở lưu trú thành công! Vui lòng chờ phê duyệt.";
                return RedirectToAction(nameof(DangKyThanhCong));
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi khi đăng ký tài khoản.");
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.");
            LoadRegisterLookups(model.PhuongXaId);
            return View(model);
        }
    }

    // GET: /account/register-success
    [HttpGet("register-success")]
    public IActionResult DangKyThanhCong()
    {
        return View("RegisterSuccess");
    }

    // GET: /account/pending-approval
    [HttpGet("pending-approval")]
    public IActionResult PendingApproval()
    {
        return View();
    }

    // POST: /account/logout
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangXuat()
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Người dùng {TenDangNhap} đã đăng xuất.", userName);
        TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
        return RedirectToAction("Index", "TrangChu");
    }

    // GET: /account/access-denied
    [HttpGet("access-denied")]
    public IActionResult TuChoiTruyCap()
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
            VaiTroConst.NguoiNuocNgoai => RedirectToAction("ThongTinCaNhan", "NguoiNuocNgoai"),
            VaiTroConst.ChuLuuTru => RedirectToAction("TongQuan", "CoSoLuuTru"),
            VaiTroConst.CanBoXNC => RedirectToAction("TongQuan", "CanBo"),
            VaiTroConst.CongAn => RedirectToAction("TongQuan", "CongAn"),
            VaiTroConst.Admin => RedirectToAction("TongQuan", "QuanTri"),
            _ => RedirectToAction("Index", "TrangChu")
        };
    }

    private void LoadRegisterLookups(int? selectedWardId = null)
    {
        var wards = _context.PhuongXas
            .OrderBy(p => p.TenPhuongXa)
            .Select(p => new
            {
                p.MaPhuongXa,
                DisplayName = p.TenPhuongXa
            })
            .ToList();

        ViewBag.PhuongXas = new SelectList(wards, "MaPhuongXa", "DisplayName", selectedWardId);
    }
}
