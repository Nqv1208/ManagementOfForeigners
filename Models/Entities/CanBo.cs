using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng CanBo - Thông tin cán bộ quản lý (Công an, Cán bộ QL XNC)
/// </summary>
[Table("CanBo")]
public class CanBo
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaCanBo { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số CCCD không được để trống")]
    [Column(TypeName = "char(12)")]
    [StringLength(12)]
    public string SoCCCD { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayCapCCCD { get; set; }

    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? NoiCapCCCD { get; set; }

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? DiaChiThuongTru { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    [StringLength(10)]
    [Column(TypeName = "nvarchar(10)")]
    public string GioiTinh { get; set; } = string.Empty;

    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? DonViCongTac { get; set; }

    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string? ChucVu { get; set; }

    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string? CapQuanLy { get; set; }

    [Required]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = "Hoạt động";

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
