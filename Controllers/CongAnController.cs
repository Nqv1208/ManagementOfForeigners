using System.Security.Claims;
using ManagementOfForeigners.Data;
using ManagementOfForeigners.Filters;
using ManagementOfForeigners.Helpers;
using ManagementOfForeigners.Models.Entities;
using ManagementOfForeigners.Models.ViewModels.CongAn;
using CanhBaoViewModel = ManagementOfForeigners.Models.ViewModels.CanBo.CanhBaoViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace ManagementOfForeigners.Controllers;

[AuthorizeRole(VaiTroConst.CongAn)]
[Route("police")]
public class CongAnController : Controller
{
    private readonly ApplicationDbContext _context;

    public CongAnController(ApplicationDbContext context)
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
        return await _context.CanBos
            .Include(c => c.PhuongXa)
            .FirstOrDefaultAsync(c => c.MaTaiKhoan == accountId);
    }

    private IQueryable<HoSoKhaiBaoTamTru> WardDeclarations(int wardId)
    {
        return _context.HoSoKhaiBaoTamTrus
            .Include(h => h.TaiKhoan)
            .ThenInclude(t => t.NguoiNuocNgoai)
            .Include(h => h.CoSoLuuTru)
            .ThenInclude(c => c.PhuongXa)
            .Where(h => h.CoSoLuuTru.MaPhuongXa == wardId);
    }

    private IQueryable<NguoiNuocNgoai> WardForeigners(int wardId)
    {
        var wardForeignerIds = _context.LichSuCuTrus
            .Where(l => l.TrangThai == TrangThaiLuuTru.DangO && l.CoSoLuuTru.MaPhuongXa == wardId)
            .Select(l => l.MaNguoiNuocNgoai)
            .Distinct();

        return _context.NguoiNuocNgoais
            .Where(n => wardForeignerIds.Contains(n.MaNguoiNuocNgoai));
    }

    // GET: /police/dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> TongQuan()
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var declarations = WardDeclarations(canBo.MaPhuongXa);
        var totalPending = await declarations.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
        var totalApproved = await declarations.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.DaDuyet);
        var foreignersInWard = await WardForeigners(canBo.MaPhuongXa).CountAsync();
        var totalWarnings = await _context.CanhBaoViPhams.CountAsync(w => w.MaCanBo == canBo.MaCanBo);

        var viewModel = new TongQuanViewModel
        {
            TenDiaBan = canBo.PhuongXa.TenPhuongXa,
            TotalPending = totalPending,
            TotalApproved = totalApproved,
            TotalForeigners = foreignersInWard,
            TotalWarnings = totalWarnings,
            RecentPending = await declarations
                .Where(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet)
                .OrderByDescending(h => h.NgayKhaiBao)
                .Take(5)
                .ToListAsync(),
            RecentWarnings = await _context.CanhBaoViPhams
                .Include(w => w.NguoiNuocNgoai)
                .Where(w => w.MaCanBo == canBo.MaCanBo)
                .OrderByDescending(w => w.NgayCanhBao)
                .Take(5)
                .ToListAsync()
        };

        return View(viewModel);
    }

    // GET: /police/declarations
    [HttpGet("declarations")]
    public async Task<IActionResult> DanhSachKhaiBao(string? search, string? trangThai, int? page)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var query = WardDeclarations(canBo.MaPhuongXa);

        var baseQuery = WardDeclarations(canBo.MaPhuongXa);
        ViewBag.CountPending = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
        ViewBag.CountApproved = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.DaDuyet);
        ViewBag.CountRejected = await baseQuery.CountAsync(h => h.TrangThai == TrangThaiKhaiBao.TuChoi);

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(h => h.TrangThai == trangThai);
        }
        else
        {
            trangThai = TrangThaiKhaiBao.ChoDuyet;
            query = query.Where(h => h.TrangThai == TrangThaiKhaiBao.ChoDuyet);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h =>
                (h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.HoTen.Contains(search)) ||
                (h.TaiKhoan.NguoiNuocNgoai != null && h.TaiKhoan.NguoiNuocNgoai.SoHoChieu.Contains(search)) ||
                h.DiaChiLuuTru.Contains(search) ||
                h.CoSoLuuTru.TenCoSo.Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        ViewBag.TenDiaBan = canBo.PhuongXa.TenPhuongXa;

        var pagedList = query.OrderByDescending(h => h.NgayKhaiBao).ToPagedList(page ?? 1, 10);
        return View(pagedList);
    }

    // GET: /police/declaration/{id}
    [HttpGet("declaration/{id}")]
    public async Task<IActionResult> ChiTietKhaiBao(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var declaration = await WardDeclarations(canBo.MaPhuongXa)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id);

        if (declaration == null)
        {
            return NotFound();
        }

        if (declaration.TaiKhoan.NguoiNuocNgoai != null)
        {
            PopulateCurrentResidence(new[] { declaration.TaiKhoan.NguoiNuocNgoai });
        }

        return View(declaration);
    }

    // POST: /police/approve
    [HttpPost("approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PheDuyet(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var declaration = await WardDeclarations(canBo.MaPhuongXa)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == id);
        if (declaration == null)
        {
            return NotFound();
        }

        if (declaration.TrangThai != TrangThaiKhaiBao.ChoDuyet)
        {
            TempData["WarningMessage"] = "Chỉ hồ sơ đang chờ duyệt mới được phê duyệt.";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id });
        }

        var foreigner = declaration.TaiKhoan.NguoiNuocNgoai;
        if (foreigner == null)
        {
            TempData["ErrorMessage"] = "Hồ sơ khai báo chưa liên kết người nước ngoài.";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            declaration.TrangThai = TrangThaiKhaiBao.DaDuyet;

            var activeStays = await _context.LichSuCuTrus
                .Where(l => l.MaNguoiNuocNgoai == foreigner.MaNguoiNuocNgoai && l.TrangThai == TrangThaiLuuTru.DangO)
                .ToListAsync();

            foreach (var stay in activeStays)
            {
                stay.TrangThai = TrangThaiLuuTru.DaRoi;
                stay.NgayKetThuc = declaration.NgayBatDau;
            }

            _context.LichSuCuTrus.Add(new LichSuCuTru
            {
                MaLSLuuTru = IdGenerator.NewMaLichSuCuTru(_context),
                MaNguoiNuocNgoai = foreigner.MaNguoiNuocNgoai,
                MaCoSoLuuTru = declaration.MaCoSoLuuTru,
                NgayBatDau = declaration.NgayBatDau,
                NgayKetThuc = declaration.NgayKetThuc,
                TrangThai = declaration.NgayKetThuc.Date < DateTime.Today ? TrangThaiLuuTru.QuaHan : TrangThaiLuuTru.DangO,
                GhiChu = $"Tạo từ hồ sơ khai báo {declaration.MaHSKhaiBao}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Không thể phê duyệt hồ sơ. Vui lòng thử lại.";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id });
        }

        TempData["SuccessMessage"] = "Phê duyệt hồ sơ khai báo tạm trú thành công.";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // POST: /police/reject
    [HttpPost("reject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(TuChoiViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối.";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id = model.MaHSKhaiBao });
        }

        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var declaration = await WardDeclarations(canBo.MaPhuongXa)
            .FirstOrDefaultAsync(h => h.MaHSKhaiBao == model.MaHSKhaiBao);
        if (declaration == null)
        {
            return NotFound();
        }

        if (declaration.TrangThai != TrangThaiKhaiBao.ChoDuyet)
        {
            TempData["WarningMessage"] = "Chỉ hồ sơ đang chờ duyệt mới được từ chối.";
            return RedirectToAction(nameof(ChiTietKhaiBao), new { id = model.MaHSKhaiBao });
        }

        declaration.TrangThai = TrangThaiKhaiBao.TuChoi;
        declaration.LyDoTuChoi = model.LyDoTuChoi;
        await _context.SaveChangesAsync();

        TempData["WarningMessage"] = $"Đã từ chối duyệt hồ sơ. Lý do: {model.LyDoTuChoi}";
        return RedirectToAction(nameof(DanhSachKhaiBao));
    }

    // GET: /police/search
    [HttpGet("search")]
    public async Task<IActionResult> TraCuu(string? search, string? quocTich, string? loaiVisa, int? page)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        ViewBag.Nationalities = await WardForeigners(canBo.MaPhuongXa).Select(n => n.QuocTich).Distinct().ToListAsync();
        ViewBag.VisaTypes = await WardForeigners(canBo.MaPhuongXa).Select(n => n.LoaiVisa).Distinct().ToListAsync();
        ViewBag.SelectedQuocTich = quocTich;
        ViewBag.SelectedLoaiVisa = loaiVisa;

        var query = WardForeigners(canBo.MaPhuongXa);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n => n.HoTen.Contains(search) || n.SoHoChieu.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(quocTich))
        {
            query = query.Where(n => n.QuocTich == quocTich);
        }

        if (!string.IsNullOrWhiteSpace(loaiVisa))
        {
            query = query.Where(n => n.LoaiVisa == loaiVisa);
        }

        ViewBag.Search = search;
        ViewBag.TenDiaBan = canBo.PhuongXa.TenPhuongXa;

        var pagedList = query.OrderBy(n => n.HoTen).ToPagedList(page ?? 1, 10);
        PopulateCurrentResidence(pagedList);
        return View(pagedList);
    }

    // GET: /police/send-warning
    [HttpGet("send-warning")]
    public async Task<IActionResult> CanhBaoViPham(string? maNguoiNuocNgoai)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        await LoadWardForeignersAsync(canBo.MaPhuongXa, maNguoiNuocNgoai);
        return View(new CanhBaoViewModel { MaNguoiNuocNgoai = maNguoiNuocNgoai ?? string.Empty });
    }

    // POST: /police/send-warning
    [HttpPost("send-warning")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CanhBaoViPham(CanhBaoViewModel model)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        if (!await WardForeigners(canBo.MaPhuongXa).AnyAsync(n => n.MaNguoiNuocNgoai == model.MaNguoiNuocNgoai))
        {
            ModelState.AddModelError(nameof(model.MaNguoiNuocNgoai), "Người nước ngoài không thuộc địa bàn phụ trách.");
        }

        if (!ModelState.IsValid)
        {
            await LoadWardForeignersAsync(canBo.MaPhuongXa, model.MaNguoiNuocNgoai);
            return View(model);
        }

        _context.CanhBaoViPhams.Add(new CanhBaoViPham
        {
            MaNguoiNuocNgoai = model.MaNguoiNuocNgoai,
            MaCanBo = canBo.MaCanBo,
            LoaiViPham = model.LoaiViPham,
            NoiDungCanhBao = model.NoiDungCanhBao,
            MucDoViPham = model.MucDoViPham,
            NgayCanhBao = DateTime.Now,
            TrangThai = TrangThaiCanhBao.DaGui,
            GhiChu = model.GhiChu
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gửi cảnh báo vi phạm thành công.";
        return RedirectToAction(nameof(TongQuan));
    }

    // GET: /police/report-violation
    [HttpGet("report-violation")]
    public async Task<IActionResult> BaoCaoViPham(string? maNguoiNuocNgoai)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        await LoadWardForeignersAsync(canBo.MaPhuongXa, maNguoiNuocNgoai);
        return View(new BaoCaoViPhamViewModel { MaNguoiNuocNgoai = maNguoiNuocNgoai ?? string.Empty });
    }

    // POST: /police/report-violation
    [HttpPost("report-violation")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BaoCaoViPham(BaoCaoViPhamViewModel model)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        if (!await WardForeigners(canBo.MaPhuongXa).AnyAsync(n => n.MaNguoiNuocNgoai == model.MaNguoiNuocNgoai))
        {
            ModelState.AddModelError(nameof(model.MaNguoiNuocNgoai), "Người nước ngoài không thuộc địa bàn phụ trách.");
        }

        if (!ModelState.IsValid)
        {
            await LoadWardForeignersAsync(canBo.MaPhuongXa, model.MaNguoiNuocNgoai);
            return View(model);
        }

        var combinedContent = $"[Loại: {model.LoaiBaoCao}] [Khẩn cấp: {model.MucDoKhanCap}] [Nội dung: {model.NoiDungBaoCao}]";
        if (!string.IsNullOrWhiteSpace(model.DeXuatXuLy))
        {
            combinedContent += $" [Kiến nghị: {model.DeXuatXuLy}]";
        }

        _context.BaoCaoViPhams.Add(new BaoCaoViPham
        {
            MaBaoCao = IdGenerator.NewMaBaoCao(_context),
            MaNguoiNuocNgoai = model.MaNguoiNuocNgoai,
            MaCanBo = canBo.MaCanBo,
            NoiDungBaoCao = combinedContent,
            NgayBaoCao = DateTime.Now,
            TrangThaiXuLy = TrangThaiXuLyConst.ChuaXuLy
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã gửi báo cáo vi phạm lên Cán bộ quản lý xuất nhập cảnh.";
        return RedirectToAction(nameof(TongQuan));
    }

    // GET: /police/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> ThongKe()
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        var wardForeignerIds = _context.LichSuCuTrus
            .Where(l => l.TrangThai == TrangThaiLuuTru.DangO && l.CoSoLuuTru.MaPhuongXa == canBo.MaPhuongXa)
            .Select(l => l.MaNguoiNuocNgoai);

        ViewBag.TenDiaBan = canBo.PhuongXa.TenPhuongXa;
        ViewBag.QuocTichStats = await _context.NguoiNuocNgoais
            .Where(n => wardForeignerIds.Contains(n.MaNguoiNuocNgoai))
            .GroupBy(n => n.QuocTich)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        ViewBag.FacilityStats = await _context.LichSuCuTrus
            .Include(l => l.CoSoLuuTru)
            .Where(l => l.TrangThai == TrangThaiLuuTru.DangO && l.CoSoLuuTru.MaPhuongXa == canBo.MaPhuongXa)
            .GroupBy(l => l.CoSoLuuTru.TenCoSo)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        return View();
    }

    // GET: /police/warnings
    [HttpGet("warnings")]
    public async Task<IActionResult> DanhSachCanhBao(string? search, string? trangThai, int? page)
    {
        var canBo = await GetCurrentCanBoAsync();
        if (canBo == null)
        {
            return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        }

        // Người nước ngoài đang cư trú trên địa bàn phụ trách
        var wardForeignerIds = WardForeigners(canBo.MaPhuongXa).Select(n => n.MaNguoiNuocNgoai);

        // Chỉ lấy cảnh báo do Cán bộ XNC ban hành, liên quan đến người nước ngoài trên địa bàn
        var query = _context.CanhBaoViPhams
            .Include(w => w.NguoiNuocNgoai)
            .Include(w => w.CanBo)
            .Where(w => wardForeignerIds.Contains(w.MaNguoiNuocNgoai) &&
                        w.CanBo.TaiKhoan.VaiTro.TenVaiTro == VaiTroConst.CanBoXNC);

        var baseQuery = query;
        ViewBag.CountSent = await baseQuery.CountAsync(w => w.TrangThai == TrangThaiCanhBao.DaGui);
        ViewBag.CountProcessing = await baseQuery.CountAsync(w => w.TrangThai == TrangThaiCanhBao.DangXuLy);
        ViewBag.CountResolved = await baseQuery.CountAsync(w => w.TrangThai == TrangThaiCanhBao.DaXuLy);

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(w => w.TrangThai == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(w =>
                w.NguoiNuocNgoai.HoTen.Contains(search) ||
                w.NguoiNuocNgoai.SoHoChieu.Contains(search) ||
                w.LoaiViPham.Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        ViewBag.TenDiaBan = canBo.PhuongXa.TenPhuongXa;

        var pagedList = query.OrderByDescending(w => w.NgayCanhBao).ToPagedList(page ?? 1, 10);
        return View(pagedList);
    }

    private async Task LoadWardForeignersAsync(int wardId, string? selectedForeignerId)
    {
        var foreigners = await WardForeigners(wardId)
            .OrderBy(n => n.HoTen)
            .Select(n => new SelectListItem
            {
                Value = n.MaNguoiNuocNgoai,
                Text = $"{n.HoTen} - Hộ chiếu: {n.SoHoChieu} ({n.QuocTich})"
            })
            .ToListAsync();

        ViewBag.NguoiNuocNgoais = new SelectList(foreigners, "Value", "Text", selectedForeignerId);
    }

    private void PopulateCurrentResidence(IEnumerable<NguoiNuocNgoai> foreigners)
    {
        var ids = foreigners.Select(f => f.MaNguoiNuocNgoai).ToList();
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
