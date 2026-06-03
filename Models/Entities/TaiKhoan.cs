using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementOfForeigners.Models.Entities;

/// <summary>
/// Bảng TaiKhoan - Tài khoản người dùng hệ thống
/// </summary>
[Table("TaiKhoan")]
public class TaiKhoan
{
    [Key]
    [Column(TypeName = "char(9)")]
    [StringLength(9)]
    public string MaTaiKhoan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string MatKhauHash { get; set; } = string.Empty;

    [Required]
    public int MaVaiTro { get; set; }

    [ForeignKey("MaVaiTro")]
    public VaiTro VaiTro { get; set; } = null!;

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Column(TypeName = "nvarchar(100)")]
    public string? Email { get; set; }

    [StringLength(15)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Column(TypeName = "nvarchar(15)")]
    public string? SoDienThoai { get; set; }

    [Required(ErrorMessage = "Trạng thái không được để trống")]
    [StringLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string TrangThai { get; set; } = "Hoạt động";

    [Column(TypeName = "datetime")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Column(TypeName = "datetime")]
    public DateTime? LanDangNhapCuoi { get; set; }

    // Navigation properties
    public NguoiNuocNgoai? NguoiNuocNgoai { get; set; }
    public CoSoLuuTru? CoSoLuuTru { get; set; }
    public CanBo? CanBo { get; set; }
    public ChuCoSoLuuTru? ChuCoSoLuuTru { get; set; }
    public ICollection<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus { get; set; } = new List<HoSoKhaiBaoTamTru>();
    public ICollection<LichSuCapNhatThongTin> LichSuCapNhats { get; set; } = new List<LichSuCapNhatThongTin>();
}

/// <summary>
/// Hằng số vai trò trong hệ thống
/// </summary>
public static class VaiTroConst
{
    public const string NguoiNuocNgoai = "NguoiNuocNgoai";
    public const string ChuLuuTru = "ChuLuuTru";
    public const string CongAn = "CongAn";
    public const string CanBoXNC = "CanBoXNC";
    public const string Admin = "Admin";
}

/// <summary>
/// Hằng số trạng thái tài khoản
/// </summary>
public static class TrangThaiTaiKhoan
{
    public const string HoatDong = "Hoạt động";
    public const string Khoa = "Khoá";
    public const string ChoDuyet = "Chờ duyệt";
}
