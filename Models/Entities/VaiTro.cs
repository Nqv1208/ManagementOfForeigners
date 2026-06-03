using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng VaiTro - Quản lý các vai trò trong hệ thống
/// </summary>
[Table("VaiTro")]
public class VaiTro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int MaVaiTro { get; set; }

    [Required(ErrorMessage = "Tên vai trò không được để trống")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string TenVaiTro { get; set; } = string.Empty;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? MoTaVaiTro { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    [Column(TypeName = "varchar(20)")]
    public string TrangThai { get; set; } = "Hoạt động";

    // Navigation properties
    public ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    public ICollection<PhanQuyenVaiTro> PhanQuyenVaiTros { get; set; } = new List<PhanQuyenVaiTro>();
}
