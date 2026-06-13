namespace ManagementOfForeigners.Models.Entities;

public class CanhBaoViPham
{
    public int MaCanhBao { get; set; }
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string MaCanBo { get; set; } = string.Empty;
    public string LoaiViPham { get; set; } = string.Empty;
    public string NoiDungCanhBao { get; set; } = string.Empty;
    public string MucDoViPham { get; set; } = string.Empty;
    public DateTime NgayCanhBao { get; set; } = DateTime.Now;
    public string TrangThai { get; set; } = "Đã gửi";
    public string? GhiChu { get; set; }

    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;
    public CanBo CanBo { get; set; } = null!;
}

public static class MucDoViPhamConst
{
    public const string Nhe = "Nhẹ";
    public const string TrungBinh = "Trung bình";
    public const string NghiemTrong = "Nghiêm trọng";
}

public static class TrangThaiCanhBao
{
    public const string DaGui = "Đã gửi";
    public const string DangXuLy = "Đang xử lý";
    public const string DaXuLy = "Đã xử lý";
}
