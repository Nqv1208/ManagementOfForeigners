namespace ManagementOfForeigners.Models.Entities;

public class NguoiNuocNgoai
{
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string? MaTaiKhoan { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string QuocTich { get; set; } = string.Empty;
    public string SoHoChieu { get; set; } = string.Empty;
    public DateTime NgayCapHoChieu { get; set; }
    public DateTime NgayHetHanHoChieu { get; set; }
    public string LoaiVisa { get; set; } = string.Empty;
    public DateTime NgayHetHanVisa { get; set; }

    public TaiKhoan? TaiKhoan { get; set; }
    public ICollection<LichSuCuTru> LichSuCuTrus { get; set; } = new List<LichSuCuTru>();
    public ICollection<CanhBaoViPham> CanhBaoViPhams { get; set; } = new List<CanhBaoViPham>();
    public ICollection<BaoCaoViPham> BaoCaoViPhams { get; set; } = new List<BaoCaoViPham>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string NoiCuTruHienTai { get; set; } = string.Empty;
}
