namespace ManagementOfForeigners.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    // ===== Thông tin tài khoản =====

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [Required(ErrorMessage = "Vai trò không được để trống")]
    [Display(Name = "Loại tài khoản")]
    public string VaiTro { get; set; } = string.Empty;

    // ===== Thông tin người nước ngoài (khi VaiTro = NguoiNuocNgoai) =====

    [Display(Name = "Họ và tên")]
    public string? HoTen { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [Display(Name = "Quốc tịch")]
    public string? QuocTich { get; set; }

    [Display(Name = "Số hộ chiếu")]
    public string? SoHoChieu { get; set; }

    [Display(Name = "Ngày cấp hộ chiếu")]
    [DataType(DataType.Date)]
    public DateTime? NgayCapHoChieu { get; set; }

    [Display(Name = "Ngày hết hạn hộ chiếu")]
    [DataType(DataType.Date)]
    public DateTime? NgayHetHanHoChieu { get; set; }

    [Display(Name = "Loại visa")]
    public string? LoaiVisa { get; set; }

    [Display(Name = "Ngày hết hạn visa")]
    [DataType(DataType.Date)]
    public DateTime? NgayHetHanVisa { get; set; }

    // ===== Thông tin cơ sở lưu trú (khi VaiTro = ChuLuuTru) =====

    [Display(Name = "Tên cơ sở lưu trú")]
    public string? TenCoSo { get; set; }

    [Display(Name = "Địa chỉ cơ sở")]
    public string? DiaChiCoSo { get; set; }

    [Display(Name = "Số điện thoại cơ sở")]
    [Phone(ErrorMessage = "Số điện thoại cơ sở không hợp lệ")]
    public string? SoDienThoaiCoSo { get; set; }

    [Display(Name = "Email cơ sở")]
    [EmailAddress(ErrorMessage = "Email cơ sở không hợp lệ")]
    public string? EmailCoSo { get; set; }
}
