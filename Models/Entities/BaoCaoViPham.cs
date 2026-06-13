namespace ManagementOfForeigners.Models.Entities;

public class BaoCaoViPham
{
    public string MaBaoCao { get; set; } = string.Empty;
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string MaCanBo { get; set; } = string.Empty;
    public string NoiDungBaoCao { get; set; } = string.Empty;
    public DateTime NgayBaoCao { get; set; } = DateTime.Now;
    public string TrangThaiXuLy { get; set; } = "Chưa xử lý";

    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;
    public CanBo CanBo { get; set; } = null!;
}

public static class TrangThaiXuLyConst
{
    public const string ChuaXuLy = "Chưa xử lý";
    public const string DangXuLy = "Đang xử lý";
    public const string DaXuLy = "Đã xử lý";
}
