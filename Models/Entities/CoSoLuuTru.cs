using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng CoSoLuuTru - Cơ sở lưu trú (khách sạn, nhà trọ...)
/// </summary>
[Table("CoSoLuuTru")]
public class CoSoLuuTru
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaCoSoLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên cơ sở không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string TenCoSo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string DiaChi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [StringLength(15)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Column(TypeName = "nvarchar(15)")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [StringLength(50)]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Column(TypeName = "varchar(50)")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = TrangThaiCoSo.DangHoatDong;

    // Navigation properties
    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; } = null!;

    public ICollection<LichSuLuuTru> LichSuLuuTrus { get; set; } = new List<LichSuLuuTru>();
    public ICollection<LichSuCuTru> LichSuCuTrus { get; set; } = new List<LichSuCuTru>();
    public ICollection<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus { get; set; } = new List<HoSoKhaiBaoTamTru>();
}

/// <summary>
/// Hằng số trạng thái cơ sở lưu trú
/// </summary>
public static class TrangThaiCoSo
{
    public const string DangHoatDong = "Đang hoạt động";
    public const string DaNgungHoatDong = "Đã ngừng hoạt động";
}
