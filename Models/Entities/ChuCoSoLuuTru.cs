namespace ManagementOfForeigners.Models.Entities;

public class ChuCoSoLuuTru
{
    public string MaChuCoSo { get; set; } = string.Empty;
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public DateTime NgayCapCCCD { get; set; }
    public string? NoiCapCCCD { get; set; }
    public string? DiaChiThuongTru { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<CoSoLuuTru> CoSoLuuTrus { get; set; } = new List<CoSoLuuTru>();
}
