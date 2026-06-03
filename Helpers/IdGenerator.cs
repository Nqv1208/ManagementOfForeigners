using ManagementOfForeigners.Data;

namespace ManagementOfForeigners.Helpers;

/// <summary>
/// Helper tạo mã ID CHAR(9) tự động theo prefix
/// Format: [Prefix][6 chữ số] ví dụ: TK000001, NN000001, HS000001
/// </summary>
public static class IdGenerator
{
    /// <summary>
    /// Tạo mã TaiKhoan mới: TK000001, TK000002...
    /// </summary>
    public static string NewMaTaiKhoan(ApplicationDbContext context)
    {
        var lastId = context.TaiKhoans
            .OrderByDescending(t => t.MaTaiKhoan)
            .Select(t => t.MaTaiKhoan)
            .FirstOrDefault();
        return GenerateNext("TK", lastId);
    }

    /// <summary>
    /// Tạo mã NguoiNuocNgoai mới: NN000001, NN000002...
    /// </summary>
    public static string NewMaNguoiNuocNgoai(ApplicationDbContext context)
    {
        var lastId = context.NguoiNuocNgoais
            .OrderByDescending(n => n.MaNguoiNuocNgoai)
            .Select(n => n.MaNguoiNuocNgoai)
            .FirstOrDefault();
        return GenerateNext("NN", lastId);
    }

    /// <summary>
    /// Tạo mã HoSoKhaiBaoTamTru mới: HS000001, HS000002...
    /// </summary>
    public static string NewMaHSKhaiBao(ApplicationDbContext context)
    {
        var lastId = context.HoSoKhaiBaoTamTrus
            .OrderByDescending(h => h.MaHSKhaiBao)
            .Select(h => h.MaHSKhaiBao)
            .FirstOrDefault();
        return GenerateNext("HS", lastId);
    }

    /// <summary>
    /// Tạo mã CoSoLuuTru mới: CS000001, CS000002...
    /// </summary>
    public static string NewMaCoSoLuuTru(ApplicationDbContext context)
    {
        var lastId = context.CoSoLuuTrus
            .OrderByDescending(c => c.MaCoSoLuuTru)
            .Select(c => c.MaCoSoLuuTru)
            .FirstOrDefault();
        return GenerateNext("CS", lastId);
    }

    /// <summary>
    /// Tạo mã LichSuCuTru mới: CT000001, CT000002...
    /// </summary>
    public static string NewMaLichSuCuTru(ApplicationDbContext context)
    {
        var lastId = context.LichSuCuTrus
            .OrderByDescending(l => l.MaLuuTru)
            .Select(l => l.MaLuuTru)
            .FirstOrDefault();
        return GenerateNext("CT", lastId);
    }

    /// <summary>
    /// Tạo mã LichSuLuuTru mới: LS000001, LS000002...
    /// </summary>
    public static string NewMaLichSuLuuTru(ApplicationDbContext context)
    {
        var lastId = context.LichSuLuuTrus
            .OrderByDescending(l => l.MaLSLuuTru)
            .Select(l => l.MaLSLuuTru)
            .FirstOrDefault();
        return GenerateNext("LS", lastId);
    }

    /// <summary>
    /// Tạo mã LichSuCapNhatThongTin mới: CN000001, CN000002...
    /// </summary>
    public static string NewMaLSCapNhat(ApplicationDbContext context)
    {
        var lastId = context.LichSuCapNhatThongTins
            .OrderByDescending(l => l.MaLSCapNhat)
            .Select(l => l.MaLSCapNhat)
            .FirstOrDefault();
        return GenerateNext("CN", lastId);
    }

    /// <summary>
    /// Tạo mã BaoCaoViPham mới: BC000001, BC000002...
    /// </summary>
    public static string NewMaBaoCao(ApplicationDbContext context)
    {
        var lastId = context.BaoCaoViPhams
            .OrderByDescending(b => b.MaBaoCao)
            .Select(b => b.MaBaoCao)
            .FirstOrDefault();
        return GenerateNext("BC", lastId);
    }

    /// <summary>
    /// Tạo mã CanBo mới: CB000001, CB000002...
    /// </summary>
    public static string NewMaCanBo(ApplicationDbContext context)
    {
        var lastId = context.CanBos
            .OrderByDescending(c => c.MaCanBo)
            .Select(c => c.MaCanBo)
            .FirstOrDefault();
        return GenerateNext("CB", lastId);
    }

    /// <summary>
    /// Tạo mã ChuCoSoLuuTru mới: CC000001, CC000002...
    /// </summary>
    public static string NewMaChuCoSo(ApplicationDbContext context)
    {
        var lastId = context.ChuCoSoLuuTrus
            .OrderByDescending(c => c.MaChuCoSo)
            .Select(c => c.MaChuCoSo)
            .FirstOrDefault();
        return GenerateNext("CC", lastId);
    }

    private static string GenerateNext(string prefix, string? lastId)
    {
        if (string.IsNullOrEmpty(lastId))
            return $"{prefix}000001";

        var numberPart = lastId.Substring(prefix.Length);
        if (int.TryParse(numberPart, out int currentNumber))
        {
            return $"{prefix}{(currentNumber + 1):D6}";
        }

        return $"{prefix}000001";
    }
}
