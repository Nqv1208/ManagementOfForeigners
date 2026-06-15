using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Models.ViewModels.NguoiNuocNgoai;

public class TongQuanViewModel
{
    public ManagementOfForeigners.Models.Entities.NguoiNuocNgoai PersonalInfo { get; set; } = null!;
    public TaiKhoan AccountInfo { get; set; } = null!;
    public List<HoSoKhaiBaoTamTru> RecentDeclarations { get; set; } = new();
    public List<LichSuCuTru> RecentResidenceHistory { get; set; } = new();
    public List<CanhBaoViPham> ActiveWarnings { get; set; } = new();
}
