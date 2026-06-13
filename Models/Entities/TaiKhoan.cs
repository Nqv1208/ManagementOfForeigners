namespace ManagementOfForeigners.Models.Entities;

public class TaiKhoan
{
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhauHash { get; set; } = string.Empty;
    public int MaVaiTro { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string TrangThai { get; set; } = "Hoạt động";
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public DateTime? LanDangNhapCuoi { get; set; }

    public VaiTro VaiTro { get; set; } = null!;
    public NguoiNuocNgoai? NguoiNuocNgoai { get; set; }
    public CanBo? CanBo { get; set; }
    public ChuCoSoLuuTru? ChuCoSoLuuTru { get; set; }
    public ICollection<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus { get; set; } = new List<HoSoKhaiBaoTamTru>();
    public ICollection<LichSuCapNhatThongTinCaNhan> LichSuCapNhats { get; set; } = new List<LichSuCapNhatThongTinCaNhan>();
}

public static class VaiTroConst
{
    public const string NguoiNuocNgoai = "NguoiNuocNgoai";
    public const string ChuLuuTru = "ChuLuuTru";
    public const string CongAn = "CongAn";
    public const string CanBoXNC = "CanBoXNC";
    public const string Admin = "Admin";
}

public static class TrangThaiTaiKhoan
{
    public const string HoatDong = "Hoạt động";
    public const string Khoa = "Khoá";
    public const string ChoDuyet = "Chờ duyệt";
}
