using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng NguoiNuocNgoai - Thông tin người nước ngoài
/// </summary>
[Table("NguoiNuocNgoai")]
public class NguoiNuocNgoai
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    [StringLength(10)]
    [Column(TypeName = "nvarchar(10)")]
    public string GioiTinh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quốc tịch không được để trống")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string QuocTich { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số hộ chiếu không được để trống")]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string SoHoChieu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày cấp hộ chiếu không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayCapHoChieu { get; set; }

    [Required(ErrorMessage = "Ngày hết hạn hộ chiếu không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayHetHanHoChieu { get; set; }

    [Required(ErrorMessage = "Loại visa không được để trống")]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string LoaiVisa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày hết hạn visa không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayHetHanVisa { get; set; }

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string NoiCuTruHienTai { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;

    public ICollection<LichSuCuTru> LichSuCuTrus { get; set; } = new List<LichSuCuTru>();
    public ICollection<LichSuLuuTru> LichSuLuuTrus { get; set; } = new List<LichSuLuuTru>();
    public ICollection<CanhBaoViPham> CanhBaoViPhams { get; set; } = new List<CanhBaoViPham>();
    public ICollection<BaoCaoViPham> BaoCaoViPhams { get; set; } = new List<BaoCaoViPham>();
}
