namespace ManagementOfForeigners.Models.Entities;

public class VaiTro
{
    public int MaVaiTro { get; set; }
    public string TenVaiTro { get; set; } = string.Empty;
    public string MoTaVaiTro { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public string TrangThai { get; set; } = "Hoạt động";

    public ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    public ICollection<QuyenHan> QuyenHans { get; set; } = new List<QuyenHan>();
}
