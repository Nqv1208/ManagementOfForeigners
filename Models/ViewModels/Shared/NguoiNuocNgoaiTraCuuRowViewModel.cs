namespace ManagementOfForeigners.Models.ViewModels.Shared;

public class NguoiNuocNgoaiTraCuuRowViewModel
{
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string SoHoChieu { get; set; } = string.Empty;
    public string QuocTich { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string LoaiVisa { get; set; } = string.Empty;
    public DateTime NgayHetHanVisa { get; set; }
    public string NoiCuTruHienTai { get; set; } = string.Empty;
}
