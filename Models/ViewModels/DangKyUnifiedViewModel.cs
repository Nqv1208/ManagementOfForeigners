using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels
{
    public class DangKyUnifiedViewModel
    {
        public string AccountType { get; set; } = "Foreigner";

        // ===== Account properties =====

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        // ===== Foreigner properties =====

        [Display(Name = "Họ và tên")]
        public string? HoTen { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Quốc tịch")]
        public string? QuocTich { get; set; }

        [Display(Name = "Số hộ chiếu")]
        public string? SoHoChieu { get; set; }

        [Display(Name = "Loại hộ chiếu")]
        public string? LoaiHoChieu { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hạn hộ chiếu")]
        public DateTime? HanHoChieu { get; set; }

        [Display(Name = "Địa chỉ liên hệ")]
        public string? DiaChiLienHe { get; set; }

        // ===== Lodging owner / Representative properties =====

        [Display(Name = "Họ tên người đại diện")]
        public string? HoTenNguoiDaiDien { get; set; }

        [Display(Name = "Số giấy tờ người đại diện")]
        public string? SoGiayToNguoiDaiDien { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh người đại diện")]
        public DateTime? NgaySinhNguoiDaiDien { get; set; }

        [Display(Name = "Giới tính người đại diện")]
        public string? GioiTinhNguoiDaiDien { get; set; }

        [Display(Name = "Chức vụ")]
        public string? ChucVu { get; set; }

        // ===== Lodging facility properties =====

        [Display(Name = "Tên cơ sở lưu trú")]
        public string? TenCoSoLuuTru { get; set; }

        [Phone(ErrorMessage = "Số điện thoại cơ sở không đúng định dạng")]
        [Display(Name = "Số điện thoại cơ sở")]
        public string? SoDienThoaiCoSo { get; set; }

        [EmailAddress(ErrorMessage = "Email cơ sở không đúng định dạng")]
        [Display(Name = "Email cơ sở")]
        public string? EmailCoSo { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string? TinhThanh { get; set; } = "Thành phố Đà Nẵng";

        [Display(Name = "Phường/Xã")]
        public int? PhuongXaId { get; set; }

        [Display(Name = "Địa chỉ cơ sở")]
        public string? DiaChiCoSo { get; set; }

        // ===== Agreement property =====

        [Required(ErrorMessage = "Bạn phải cam kết thông tin khai báo là đúng sự thật")]
        [MustBeTrue(ErrorMessage = "Bạn phải cam kết thông tin khai báo là đúng sự thật")]
        [Display(Name = "Cam kết thông tin")]
        public bool CamKetThongTin { get; set; }
    }

    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }
    }
}
