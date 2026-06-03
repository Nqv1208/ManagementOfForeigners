using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng CanhBaoViPham - Cảnh báo vi phạm của người nước ngoài
/// </summary>
[Table("CanhBaoViPham")]
public class CanhBaoViPham
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaCanhBao { get; set; }

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaCanBo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại vi phạm không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string LoaiViPham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung cảnh báo không được để trống")]
    [Column(TypeName = "nvarchar(max)")]
    public string NoiDungCanhBao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mức độ vi phạm không được để trống")]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string MucDoViPham { get; set; } = string.Empty;

    [Column(TypeName = "datetime")]
    public DateTime NgayCanhBao { get; set; } = DateTime.Now;

    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = TrangThaiCanhBao.DaGui;

    [Column(TypeName = "nvarchar(max)")]
    public string? GhiChu { get; set; }

    // Navigation properties
    [ForeignKey("MaNguoiNuocNgoai")]
    public NguoiNuocNgoai NguoiNuocNgoai { get; set; } = null!;

    [ForeignKey("MaCanBo")]
    public CanBo CanBo { get; set; } = null!;
}

/// <summary>
/// Hằng số mức độ vi phạm
/// </summary>
public static class MucDoViPhamConst
{
    public const string Nhe = "Nhẹ";
    public const string TrungBinh = "Trung bình";
    public const string NghiemTrong = "Nghiêm trọng";
}

/// <summary>
/// Hằng số trạng thái cảnh báo
/// </summary>
public static class TrangThaiCanhBao
{
    public const string DaGui = "Đã gửi";
    public const string DangXuLy = "Đang xử lý";
    public const string DaXuLy = "Đã xử lý";
}
