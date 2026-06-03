using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng LichSuCuTru - Lịch sử lưu trú tại cơ sở (Table 9 defined in schema.md)
/// </summary>
[Table("LichSuCuTru")]
public class LichSuLuuTru
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaLSLuuTru { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaCoSoLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayKetThuc { get; set; }

    [Required(ErrorMessage = "Phòng không được để trống")]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string Phong { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = TrangThaiLuuTru.DangO;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? GhiChu { get; set; }

    // Navigation properties
    [ForeignKey("MaNguoiNuocNgoai")]
    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;

    [ForeignKey("MaCoSoLuuTru")]
    public CoSoLuuTru CoSoLuuTru { get; set; } = null!;
}

/// <summary>
/// Hằng số trạng thái lưu trú
/// </summary>
public static class TrangThaiLuuTru
{
    public const string DangO = "Đang ở";
    public const string DaRoi = "Đã rời";
    public const string QuaHan = "Quá hạn";
}
