using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng HoSoKhaiBaoTamTru - Hồ sơ khai báo tạm trú
/// </summary>
[Table("HoSoKhaiBaoTamTru")]
public class HoSoKhaiBaoTamTru
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaHSKhaiBao { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayKhaiBao { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayBatDau { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime NgayKetThuc { get; set; }

    [Required(ErrorMessage = "Mục đích lưu trú không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string MucDichLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ lưu trú không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string DiaChiLuuTru { get; set; } = string.Empty;

    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string? MaCoSoLuuTru { get; set; }

    [Required]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = TrangThaiKhaiBao.ChoDuyet;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? LyDoTuChoi { get; set; }

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? GhiChu { get; set; }

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;

    [ForeignKey("MaCoSoLuuTru")]
    public CoSoLuuTru? CoSoLuuTru { get; set; }
}

/// <summary>
/// Hằng số trạng thái khai báo tạm trú
/// </summary>
public static class TrangThaiKhaiBao
{
    public const string ChoDuyet = "Chờ duyệt";
    public const string DaDuyet = "Đã duyệt";
    public const string TuChoi = "Từ chối";
}
