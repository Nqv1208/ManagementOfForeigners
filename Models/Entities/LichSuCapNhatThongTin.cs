using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng LichSuCapNhatThongTinCaNhan - Lịch sử cập nhật thông tin cá nhân
/// </summary>
[Table("LichSuCapNhatThongTinCaNhan")]
public class LichSuCapNhatThongTin
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaLSCapNhat { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Trường cập nhật không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string TruongCapNhat { get; set; } = string.Empty;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? GiaTriCu { get; set; }

    [Required(ErrorMessage = "Giá trị mới không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string GiaTriMoi { get; set; } = string.Empty;

    [Column(TypeName = "datetime")]
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? LyDoCapNhat { get; set; }

    [StringLength(25)]
    [Column(TypeName = "nvarchar(25)")]
    public string? TrangThai { get; set; }

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
