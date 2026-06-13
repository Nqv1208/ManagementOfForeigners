namespace ManagementOfForeigners.Models.Entities;

public class QuyenHan
{
    public int MaQuyen { get; set; }
    public string TenQuyen { get; set; } = string.Empty;
    public string MoTaQuyen { get; set; } = string.Empty;
    public int MaVaiTro { get; set; }
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;

    public VaiTro VaiTro { get; set; } = null!;
}
