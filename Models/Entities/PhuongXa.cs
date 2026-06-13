namespace ManagementOfForeigners.Models.Entities;

public class PhuongXa
{
    public int MaPhuongXa { get; set; }
    public string TenPhuongXa { get; set; } = string.Empty;

    public ICollection<CanBo> CanBos { get; set; } = new List<CanBo>();
    public ICollection<CoSoLuuTru> CoSoLuuTrus { get; set; } = new List<CoSoLuuTru>();
}
