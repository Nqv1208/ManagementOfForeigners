using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.Police;
using ManagementOfForeigners.Models.ViewModels.Officer;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.CongAn)]
public class PoliceController : Controller
{
    private readonly ApplicationDbContext _context;

    public PoliceController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentAccountId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    private string GetManagedWard()
    {
        var username = GetCurrentUsername();
        if (username.Equals("congan01", StringComparison.OrdinalIgnoreCase))
        {
            return "Hải Châu";
        }
        else if (username.Equals("congan02", StringComparison.OrdinalIgnoreCase))
        {
            return "Sơn Trà";
        }
        return "Hải Châu";
    }

    // GET: Police/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var ward = GetManagedWard();
        
        // Lấy tất cả hồ sơ tạm trú thuộc địa bàn phụ trách
        var wardDeclarationsQuery = _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .Where(h => h.DiaChiLuuTru.Contains(ward) || (h.CoSoLuuTru != null && h.CoSoLuuTru.DiaChi.Contains(ward)));

        var totalPending = await wardDeclarationsQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
        var totalApproved = await wardDeclarationsQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.DaDuyet);

        // Số lượng người nước ngoài thực tế đang ở tại địa bàn thông qua các hồ sơ đã được duyệt hoặc lịch sử cư trú
        var foreignersInWard = await _context.NguoiNuocNgoais
            .Where(n => n.NoiCuTruHienTai.Contains(ward))
            .CountAsync();

        // Số cảnh báo gửi bởi công an phường này
        var totalWarnings = await _context.CanhBaoViPhams
            .Where(w => w.MaCanBo == GetCurrentAccountId())
            .CountAsync();

        var recentPending = await wardDeclarationsQuery
            .Where(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet)
            .OrderByDescending(h => h.NgayKhaiBao)
            .Take(5)
            .ToListAsync();

        var recentWarnings = await _context.CanhBaoViPhams
            .Include(w => w.NguoiNuocNgoai)
            .Where(w => w.MaCanBo == GetCurrentAccountId())
            .OrderByDescending(w => w.NgayCanhBao)
            .Take(5)
            .ToListAsync();

        var viewModel = new ManagementOfForeigners.Models.ViewModels.Police.DashboardViewModel
        {
            TenDiaBan = ward,
            TotalPending = totalPending,
            TotalApproved = totalApproved,
            TotalForeigners = foreignersInWard,
            TotalWarnings = totalWarnings,
            RecentPending = recentPending,
            RecentWarnings = recentWarnings
        };

        return View(viewModel);
    }

    // GET: Police/DanhSachKhaiBao
    public async Task<IActionResult> DanhSachKhaiBao(string? search, string? trangThai, int? page)
    {
        var ward = GetManagedWard();
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .Where(h => h.DiaChiLuuTru.Contains(ward) || (h.CoSoLuuTru != null && h.CoSoLuuTru.DiaChi.Contains(ward)));

        if (!string.IsNullOrEmpty(trangThai))
        {
            query = query.Where(h => h.TrangThai == trangThai);
        }
        else
        {
            // Mặc định hiển thị hồ sơ chờ duyệt
            query = query.Where(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
            trangThai = TrangThaiKhaiBao.ChoDuyet;
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(h => (h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.HoTen.Contains(search)) || 
                                     (h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.SoHoChieu.Contains(search)) || 
                                     h.DiaChiLuuTru.Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        ViewBag.TenDiaBan = ward;

        var pagedList = query.OrderByDescending(h => h.NgayKhaiBao).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // GET: Police/ChiTietKhaiBao
    public async Task<IActionResult> ChiTietKhaiBao(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var ward = GetManagedWard();
        var declaration = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id);

        if (declaration == null) return NotFound();

        // Kiểm tra quyền địa bàn
        if (!declaration.DiaChiLuuTru.Contains(ward) && 
            (declaration.CoSoLuuTru == null || !declaration.CoSoLuuTru.DiaChi.Contains(ward)))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        return View(declaration);
    }

    // POST: Police/PheDuyet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PheDuyet(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var ward = GetManagedWard();
        var dec = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id);

        if (dec == null) return NotFound();

        // Kiểm tra quyền địa bàn
        if (!dec.DiaChiLuuTru.Contains(ward) && 
            (dec.CoSoLuuTru == null || !dec.CoSoLuuTru.DiaChi.Contains(ward)))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        dec.TrangThai = TrangThaiKhaiBao.DaDuyet;

        // Cập nhật nơi cư trú hiện tại của người nước ngoài
        var foreigner = dec.TaiKhoan.NguoiNuocNgoai;
        if (foreigner != null)
        {
            foreigner.NoiCuTruHienTai = dec.DiaChiLuuTru;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Phê duyệt hồ sơ khai báo tạm trú thành công!";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // POST: Police/TuChoi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(TuChoiViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối!";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id = model.MaHSKhaiBao });
        }

        var ward = GetManagedWard();
        var dec = await _context.HoSoKhaiBaoTamTrus
            .Include(h => h.CoSoLuuTru)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == model.MaHSKhaiBao);

        if (dec == null) return NotFound();

        // Kiểm tra quyền địa bàn
        if (!dec.DiaChiLuuTru.Contains(ward) && 
            (dec.CoSoLuuTru == null || !dec.CoSoLuuTru.DiaChi.Contains(ward)))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        dec.TrangThai = TrangThaiKhaiBao.TuChoi;
        dec.LyDoTuChoi = model.LyDoTuChoi;

        await _context.SaveChangesAsync();

        TempData["WarningMessage"] = $"Đã từ chối duyệt hồ sơ. Lý do: {model.LyDoTuChoi}";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // GET: Police/TraCuu
    public async Task<IActionResult> TraCuu(string? search, int? page)
    {
        var ward = GetManagedWard();
        int pageSize = 10;
        int pageNumber = page ?? 1;

        // Chỉ tra cứu những người nước ngoài cư trú trong địa bàn phụ trách
        var query = _context.NguoiNuocNgoais
            .Where(n => n.NoiCuTruHienTai.Contains(ward));

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.HoTen.Contains(search) || 
                                     n.SoHoChieu.Contains(search) || 
                                     n.NoiCuTruHienTai.Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TenDiaBan = ward;

        var pagedList = query.OrderBy(n => n.HoTen).ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    // GET: Police/CanhBaoViPham
    public async Task<IActionResult> CanhBaoViPham(string? maNguoiNuocNgoai)
    {
        var ward = GetManagedWard();
        // Chỉ chọn người nước ngoài cư trú trong địa bàn
        var foreigners = await _context.NguoiNuocNgoais
            .Where(n => n.NoiCuTruHienTai.Contains(ward))
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

    // POST: Police/CanhBaoViPham
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CanhBaoViPham(CanhBaoViewModel model)
    {
        var ward = GetManagedWard();
        if (!ModelState.IsValid)
        {
            var foreigners = await _context.NguoiNuocNgoais
                .Where(n => n.NoiCuTruHienTai.Contains(ward))
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

        TempData["SuccessMessage"] = "Gửi cảnh báo vi phạm thành công!";
        return RedirectToAction(nameof(Dashboard));
    }

    // GET: Police/BaoCaoViPham
    public async Task<IActionResult> BaoCaoViPham(string? maNguoiNuocNgoai)
    {
        var ward = GetManagedWard();
        // Chỉ chọn người nước ngoài cư trú trong địa bàn
        var foreigners = await _context.NguoiNuocNgoais
            .Where(n => n.NoiCuTruHienTai.Contains(ward))
            .Select(n => new SelectListItem
            {
                Value = n.MaNguoiNuocNgoai,
                Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
            }).ToListAsync();

        ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text", maNguoiNuocNgoai);

        var model = new BaoCaoViPhamViewModel
        {
            MaNguoiNuocNgoai = maNguoiNuocNgoai ?? string.Empty
        };

        return View(model);
    }

    // POST: Police/BaoCaoViPham
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BaoCaoViPham(BaoCaoViPhamViewModel model)
    {
        var ward = GetManagedWard();
        if (!ModelState.IsValid)
        {
            var foreigners = await _context.NguoiNuocNgoais
                .Where(n => n.NoiCuTruHienTai.Contains(ward))
                .Select(n => new SelectListItem
                {
                    Value = n.MaNguoiNuocNgoai,
                    Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
                }).ToListAsync();
            ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text", model.MaNguoiNuocNgoai);
            return View(model);
        }

        var report = new BaoCaoViPham
        {
            MaBaoCao = IdGenerator.NewMaBaoCao(_context),
            MaNguoiNuocNgoai = model.MaNguoiNuocNgoai,
            NoiDungBaoCao = model.NoiDungBaoCao,
            NgayBaoCao = DateTime.Now,
            MaCanBo = GetCurrentAccountId(),
            TrangThaiXuLy = TrangThaiXuLyConst.ChuaXuLy
        };

        _context.BaoCaoViPhams.Add(report);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã gửi báo cáo vi phạm lên Cán bộ quản lý xuất nhập cảnh thành công!";
        return RedirectToAction(nameof(Dashboard));
    }

    // GET: Police/ThongKe
    public async Task<IActionResult> ThongKe()
    {
        var ward = GetManagedWard();
        ViewBag.TenDiaBan = ward;

        // Thống kê người nước ngoài theo quốc tịch trong địa bàn
        var quocTichStats = await _context.NguoiNuocNgoais
            .Where(n => n.NoiCuTruHienTai.Contains(ward))
            .GroupBy(n => n.QuocTich)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        ViewBag.QuocTichStats = quocTichStats;

        // Thống kê theo loại hình cơ sở lưu trú (Khách sạn vs Nhà trọ)
        var facilityStats = await _context.LichSuLuuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.TrangThai == TrangThaiLuuTru.DangO && l.CoSoLuuTru.DiaChi.Contains(ward))
            .GroupBy(l => l.CoSoLuuTru.TenCoSo)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        ViewBag.FacilityStats = facilityStats;

        return View();
    }
}
