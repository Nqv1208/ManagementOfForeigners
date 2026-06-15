namespace ManagementOfForeigners.Models.Entities;

public class CoSoLuuTru
{
    public string MaCoSoLuuTru { get; set; } = string.Empty;
    public int MaPhuongXa { get; set; }
    public string MaChuCoSo { get; set; } = string.Empty;
    public string TenCoSo { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Đang hoạt động";
    public string? LoaiHinh { get; set; }
    public string? MaSoKinhDoanh { get; set; }
    public int? SoPhong { get; set; }
    public int? SucChuaToiDa { get; set; }
    public string? GhiChu { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ChuCoSoLuuTru ChuCoSoLuuTru { get; set; } = null!;
    public PhuongXa PhuongXa { get; set; } = null!;
    public ICollection<LichSuCuTru> LichSuCuTrus { get; set; } = new List<LichSuCuTru>();
    public ICollection<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus { get; set; } = new List<HoSoKhaiBaoTamTru>();
}

public static class TrangThaiCoSo
{
    public const string DangHoatDong = "Đang hoạt động";
    public const string DaNgungHoatDong = "Đã ngừng hoạt động";
    public const string ChoDuyet = "Chờ duyệt";
}
