using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng ChuCoSoLuuTru - Thông tin chủ cơ sở lưu trú
/// </summary>
[Table("ChuCoSoLuuTru")]
public class ChuCoSoLuuTru
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaChuCoSo { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string HoTen { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    [StringLength(10)]
    [Column(TypeName = "nvarchar(10)")]
    public string GioiTinh { get; set; } = string.Empty;

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

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
