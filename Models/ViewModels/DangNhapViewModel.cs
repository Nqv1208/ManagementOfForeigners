using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels;

public class DangNhapViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool GhiNho { get; set; }
}
