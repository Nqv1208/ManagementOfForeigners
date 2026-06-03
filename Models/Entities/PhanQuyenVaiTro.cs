using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng PhanQuyenVaiTro - Quản lý phân quyền chi tiết cho từng vai trò
/// </summary>
[Table("PhanQuyenVaiTro")]
public class PhanQuyenVaiTro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaQuyen { get; set; }

    [Required(ErrorMessage = "Tên quyền không được để trống")]
    [StringLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string TenQuyen { get; set; } = string.Empty;

    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? MoTaQuyen { get; set; }

    [Required]
    public int MaVaiTro { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("MaVaiTro")]
    public VaiTro VaiTro { get; set; } = null!;
}
