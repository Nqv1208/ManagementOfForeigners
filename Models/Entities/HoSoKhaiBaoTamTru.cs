namespace ManagementOfForeigners.Models.Entities;

public class HoSoKhaiBaoTamTru
{
    public string MaHSKhaiBao { get; set; } = string.Empty;
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string MaCoSoLuuTru { get; set; } = string.Empty;
    public DateTime NgayKhaiBao { get; set; } = DateTime.Now;
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string MucDichLuuTru { get; set; } = string.Empty;
    public string DiaChiLuuTru { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Chờ duyệt";
    public string? LyDoTuChoi { get; set; }
    public string? GhiChu { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public CoSoLuuTru CoSoLuuTru { get; set; } = null!;
}

public static class TrangThaiKhaiBao
{
    public const string ChoDuyet = "Chờ duyệt";
    public const string DaDuyet = "Đã duyệt";
    public const string TuChoi = "Từ chối";
}
