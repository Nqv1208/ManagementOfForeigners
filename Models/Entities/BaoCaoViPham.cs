using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng BaoCaoViPham - Báo cáo vi phạm lên cán bộ quản lý xuất nhập cảnh
/// </summary>
[Table("BaoCaoViPham")]
public class BaoCaoViPham
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaBaoCao { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung báo cáo không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string NoiDungBaoCao { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime NgayBaoCao { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaCanBo { get; set; } = string.Empty;

    [StringLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string TrangThaiXuLy { get; set; } = TrangThaiXuLyConst.ChuaXuLy;

    // Navigation properties
    [ForeignKey("MaNguoiNuocNgoai")]
    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;

    [ForeignKey("MaCanBo")]
    public CanBo CanBo { get; set; } = null!;
}

/// <summary>
/// Hằng số trạng thái xử lý báo cáo
/// </summary>
public static class TrangThaiXuLyConst
{
    public const string ChuaXuLy = "Chưa xử lý";
    public const string DangXuLy = "Đang xử lý";
    public const string DaXuLy = "Đã xử lý";
}
