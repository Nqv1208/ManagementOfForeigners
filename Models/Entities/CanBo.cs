namespace ManagementOfForeigners.Models.Entities;

public class CanBo
{
    public string MaCanBo { get; set; } = string.Empty;
    public int MaPhuongXa { get; set; }
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public DateTime NgayCapCCCD { get; set; }
    public string? NoiCapCCCD { get; set; }
    public string? DiaChiThuongTru { get; set; }
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string? DonViCongTac { get; set; }
    public string? ChucVu { get; set; }
    public string? CapQuanLy { get; set; }
    public string TrangThai { get; set; } = "Hoạt động";

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public PhuongXa PhuongXa { get; set; } = null!;
    public ICollection<CanhBaoViPham> CanhBaoViPhams { get; set; } = new List<CanhBaoViPham>();
    public ICollection<BaoCaoViPham> BaoCaoViPhams { get; set; } = new List<BaoCaoViPham>();
}
