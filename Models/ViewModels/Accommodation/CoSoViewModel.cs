using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.Accommodation;

public class CoSoViewModel
{
    [Required(ErrorMessage = "Tên cơ sở lưu trú không được để trống")]
    [StringLength(255)]
    [Display(Name = "Tên cơ sở lưu trú")]
    public string TenCoSo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ cơ sở không được để trống")]
    [StringLength(255)]
    [Display(Name = "Địa chỉ cụ thể")]
    public string DiaChi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [StringLength(15)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [StringLength(50)]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email liên hệ")]
    public string Email { get; set; } = string.Empty;
}
