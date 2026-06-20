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
    public async Task<IActionResult> TraCuu(string? search, string? quocTich, string? loaiVisa,
        string? phuongXa, string? coSoLuuTru, string? trangThaiCuTru, string? fromDate, string? toDate, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        // Bộ lọc cấp người nước ngoài (chạy trên SQL)
        var query = _context.NguoiNuocNgoais.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n => n.HoTen.Contains(search) ||
                                     n.SoHoChieu.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(quocTich))
        {
            query = query.Where(n => n.QuocTich == quocTich);
        }

        if (!string.IsNullOrWhiteSpace(loaiVisa))
        {
            query = query.Where(n => n.LoaiVisa == loaiVisa);
        }

        var foreigners = await query.OrderBy(n => n.HoTen).ToListAsync();
        var ids = foreigners.Select(n => n.MaNguoiNuocNgoai).ToList();

        // Lấy lịch sử cư trú gần nhất của từng người để hiển thị & lọc theo nơi cư trú
        var stays = await _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .ThenInclude(c => c.PhuongXa)
            .Where(l => ids.Contains(l.MaNguoiNuocNgoai))
            .ToListAsync();

        var latestStay = stays
            .GroupBy(l => l.MaNguoiNuocNgoai)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(l => l.NgayBatDau).First());

        var pairs = foreigners.Select(n =>
        {
            latestStay.TryGetValue(n.MaNguoiNuocNgoai, out var stay);
            return (Foreigner: n, Stay: stay);
        });

        // Bộ lọc theo nơi cư trú (chạy trong bộ nhớ trên bản ghi cư trú gần nhất)
        if (!string.IsNullOrWhiteSpace(phuongXa))
        {
            pairs = pairs.Where(p => p.Stay?.CoSoLuuTru?.PhuongXa?.TenPhuongXa == phuongXa);
        }

        if (!string.IsNullOrWhiteSpace(coSoLuuTru))
        {
            pairs = pairs.Where(p => p.Stay?.CoSoLuuTru != null &&
                                     p.Stay.CoSoLuuTru.TenCoSo.Contains(coSoLuuTru));
        }

        if (!string.IsNullOrWhiteSpace(trangThaiCuTru))
        {
            pairs = pairs.Where(p => p.Stay?.TrangThai == trangThaiCuTru);
        }

        if (DateTime.TryParse(fromDate, out var fd))
        {
            pairs = pairs.Where(p => p.Stay != null && p.Stay.NgayBatDau >= fd);
        }

        if (DateTime.TryParse(toDate, out var td))
        {
            pairs = pairs.Where(p => p.Stay != null && p.Stay.NgayBatDau <= td);
        }

        var rows = pairs.Select(p => new CanBoForeignerRowViewModel
        {
            MaNguoiNuocNgoai = p.Foreigner.MaNguoiNuocNgoai,
            HoTen = p.Foreigner.HoTen,
            SoHoChieu = p.Foreigner.SoHoChieu,
            QuocTich = p.Foreigner.QuocTich,
            GioiTinh = p.Foreigner.GioiTinh,
            LoaiVisa = p.Foreigner.LoaiVisa,
            NgayHetHanVisa = p.Foreigner.NgayHetHanVisa,
            TenCoSoLuuTru = p.Stay?.CoSoLuuTru?.TenCoSo ?? "—",
            TenPhuongXa = p.Stay?.CoSoLuuTru?.PhuongXa?.TenPhuongXa ?? "—",
            DiaChiCuTru = p.Stay?.CoSoLuuTru?.DiaChi ?? "—",
            TrangThaiCuTru = p.Stay?.TrangThai ?? "Ngoại tuyến"
        }).ToList();

        // Dữ liệu cho các bộ lọc
        ViewBag.Nationalities = await _context.NguoiNuocNgoais
            .Select(n => n.QuocTich)
            .Distinct()
            .ToListAsync();

        ViewBag.VisaTypes = await _context.NguoiNuocNgoais
            .Select(n => n.LoaiVisa)
            .Distinct()
            .ToListAsync();

        ViewBag.Wards = await _context.PhuongXas
            .Select(p => p.TenPhuongXa)
            .Distinct()
            .ToListAsync();

        var viewModel = new CanBoTraCuuViewModel
        {
            Search = search,
            QuocTich = quocTich,
            LoaiVisa = loaiVisa,
            PhuongXa = phuongXa,
            CoSoLuuTru = coSoLuuTru,
            TrangThaiCuTru = trangThaiCuTru,
            FromDate = fromDate,
            ToDate = toDate,
            Foreigners = rows.ToPagedList(pageNumber, pageSize)
        };

        return View(viewModel);
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
        return RedirectToAction(nameof(ViPhamCanhBao));
    }

    // GET: /officer/warnings
    [HttpGet("warnings")]
    public async Task<IActionResult> ViPhamCanhBao(string? search, int? page)
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

    // POST: /officer/update-warning-status
    [HttpPost("update-warning-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThaiCanhBao(int id, string trangThai)
    {
        var allowedStatuses = new[] { TrangThaiCanhBao.DaGui, TrangThaiCanhBao.DangXuLy, TrangThaiCanhBao.DaXuLy };
        if (!allowedStatuses.Contains(trangThai))
        {
            TempData["ErrorMessage"] = "Trạng thái cảnh báo không hợp lệ.";
            return RedirectToAction(nameof(ViPhamCanhBao));
        }

        var warning = await _context.CanhBaoViPhams.FirstOrDefaultAsync(c => c.MaCanhBao == id);
        if (warning == null)
        {
            return NotFound();
        }

        warning.TrangThai = trangThai;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật trạng thái cảnh báo vi phạm thành công.";
        return RedirectToAction(nameof(ViPhamCanhBao));
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

    // GET: /officer/residence-records
    [HttpGet("residence-records")]
    public async Task<IActionResult> HoSoTamTru(string? search, string? status, int? page)
    {
        var query = _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h =>
                (h.TaiKhoan != null && h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.HoTen.Contains(search)) ||
                (h.TaiKhoan != null && h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.SoHoChieu.Contains(search)) ||
                (h.CoSoLuuTru != null && h.CoSoLuuTru.TenCoSo.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(h => h.TrangThai == status);
        }

        ViewBag.Search = search;
        ViewBag.Status = status;
        var pagedList = query.OrderByDescending(h => h.NgayKhaiBao).ToPagedList(page ?? 1, 10);
        return View(pagedList);
    }

    // POST: /officer/save-declaration-status
    [HttpPost("save-declaration-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuTrangThaiKhaiBao(string id, string trangThai, string? ghiChu)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var allowedStatuses = new[] { "Chờ duyệt", "Cần rà soát", "Đã xử lý", "Sai thông tin" };
        if (!allowedStatuses.Contains(trangThai))
        {
            TempData["ErrorMessage"] = "Trạng thái hồ sơ không hợp lệ.";
            return RedirectToAction(nameof(HoSoTamTru));
        }

        var declaration = await _context.HoSoKhaiBaoTamTrus.FirstOrDefaultAsync(h => h.MaHSKhaiBao == id);
        if (declaration == null)
        {
            return NotFound();
        }

        declaration.TrangThai = trangThai;
        declaration.GhiChu = ghiChu;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật trạng thái hồ sơ khai báo tạm trú thành công.";
        return RedirectToAction(nameof(HoSoTamTru));
    }

    // GET: /officer/facilities
    [HttpGet("facilities")]
    public async Task<IActionResult> CoSoLuuTru(string? search, int? page)
    {
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var query = _context.CoSoLuuTrus
            .Include(c => c.PhuongXa)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.TenCoSo.Contains(search) ||
                                     c.DiaChi.Contains(search) ||
                                     c.PhuongXa.TenPhuongXa.Contains(search));
        }

        var facilities = await query.OrderBy(c => c.TenCoSo).ToListAsync();
        var facilityIds = facilities.Select(f => f.MaCoSoLuuTru).ToList();

        // Số khách đang ở theo từng cơ sở
        var occupancyMap = (await _context.LichSuCuTrus
            .Where(l => facilityIds.Contains(l.MaCoSoLuuTru) && l.TrangThai == TrangThaiLuuTru.DangO)
            .GroupBy(l => l.MaCoSoLuuTru)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        // Tổng hồ sơ khai báo theo từng cơ sở
        var declarationMap = (await _context.HoSoKhaiBaoTamTrus
            .Where(h => facilityIds.Contains(h.MaCoSoLuuTru))
            .GroupBy(h => h.MaCoSoLuuTru)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        // Tổng vi phạm: cảnh báo của những người từng cư trú tại cơ sở.
        // Lấy cặp (cơ sở, người nước ngoài) duy nhất rồi cộng số cảnh báo của từng người.
        var stayPairs = await _context.LichSuCuTrus
            .Where(l => facilityIds.Contains(l.MaCoSoLuuTru))
            .Select(l => new { l.MaCoSoLuuTru, l.MaNguoiNuocNgoai })
            .Distinct()
            .ToListAsync();

        var foreignerIds = stayPairs.Select(p => p.MaNguoiNuocNgoai).Distinct().ToList();

        var warningCountByForeigner = (await _context.CanhBaoViPhams
            .Where(c => foreignerIds.Contains(c.MaNguoiNuocNgoai))
            .GroupBy(c => c.MaNguoiNuocNgoai)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync())
            .ToDictionary(x => x.Key, x => x.Count);

        var violationMap = stayPairs
            .GroupBy(p => p.MaCoSoLuuTru)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(p => warningCountByForeigner.TryGetValue(p.MaNguoiNuocNgoai, out var cnt) ? cnt : 0));

        var rows = facilities.Select(f =>
        {
            var tongViPham = violationMap.TryGetValue(f.MaCoSoLuuTru, out var v) ? v : 0;
            return new CanBoFacilityRowViewModel
            {
                MaCoSoLuuTru = f.MaCoSoLuuTru,
                TenCoSo = f.TenCoSo,
                LoaiHinh = string.Empty,
                PhuongXa = f.PhuongXa?.TenPhuongXa ?? string.Empty,
                SoKhachDangO = occupancyMap.TryGetValue(f.MaCoSoLuuTru, out var o) ? o : 0,
                TongHoSo = declarationMap.TryGetValue(f.MaCoSoLuuTru, out var d) ? d : 0,
                TongViPham = tongViPham,
                TrangThai = f.TrangThai,
                MucRuiRo = tongViPham >= 5 ? "Cao" : tongViPham >= 1 ? "Trung bình" : "Thấp"
            };
        }).ToList();

        ViewBag.Search = search;
        var pagedList = rows.ToPagedList(pageNumber, pageSize);
        return View(pagedList);
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
