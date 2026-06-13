namespace ManagementOfForeigners.Models.Entities;

public class LichSuCapNhatThongTinCaNhan
{
    public string MaLSCapNhat { get; set; } = string.Empty;
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string TruongCapNhat { get; set; } = string.Empty;
    public string? GiaTriCu { get; set; }
    public string GiaTriMoi { get; set; } = string.Empty;
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    public string? LyDoCapNhat { get; set; }
    public string? TrangThai { get; set; }

    public TaiKhoan TaiKhoan { get; set; } = null!;
}
