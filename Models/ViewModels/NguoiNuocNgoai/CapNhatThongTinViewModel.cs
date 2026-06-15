using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.NguoiNuocNgoai;

public class CapNhatThongTinViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    [StringLength(10)]
    [Display(Name = "Giới tính")]
    public string GioiTinh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quốc tịch không được để trống")]
    [StringLength(50)]
    [Display(Name = "Quốc tịch")]
    public string QuocTich { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số hộ chiếu không được để trống")]
    [StringLength(20)]
    [Display(Name = "Số hộ chiếu")]
    public string SoHoChieu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày cấp hộ chiếu không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày cấp hộ chiếu")]
    public DateTime NgayCapHoChieu { get; set; }

    [Required(ErrorMessage = "Ngày hết hạn hộ chiếu không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày hết hạn hộ chiếu")]
    public DateTime NgayHetHanHoChieu { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email liên hệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lý do cập nhật thông tin không được để trống")]
    [StringLength(225, ErrorMessage = "Lý do không dài quá 225 ký tự")]
    [Display(Name = "Lý do thay đổi thông tin")]
    public string LyDoCapNhat { get; set; } = string.Empty;
}
