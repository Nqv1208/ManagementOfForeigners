namespace ManagementOfForeigners.Models.Entities;

public class LichSuCuTru
{
    public string MaLSLuuTru { get; set; } = string.Empty;
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string MaCoSoLuuTru { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public string? Phong { get; set; }
    public string TrangThai { get; set; } = "Đang ở";
    public string? GhiChu { get; set; }

    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;
    public CoSoLuuTru CoSoLuuTru { get; set; } = null!;
}

public static class TrangThaiLuuTru
{
    public const string DangO = "Đang ở";
    public const string DaRoi = "Đã rời";
    public const string QuaHan = "Quá hạn";
}
