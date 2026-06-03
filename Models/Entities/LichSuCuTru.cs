using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng LichSuCuTruCaNhan - Lịch sử cư trú cá nhân do người nước ngoài tự khai báo
/// </summary>
[Table("LichSuCuTruCaNhan")]
public class LichSuCuTru
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaLuuTru { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string? MaCoSoLuuTru { get; set; }

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
    public CoSoLuuTru? CoSoLuuTru { get; set; }
}
