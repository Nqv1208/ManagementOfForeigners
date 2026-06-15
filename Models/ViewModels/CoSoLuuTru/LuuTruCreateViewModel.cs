using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.CoSoLuuTru
{
    public class LuuTruCreateViewModel
    {
        // ===== Thông tin cơ sở (Readonly) =====
        public string MaCoSoLuuTru { get; set; } = string.Empty;
        public string TenCoSoLuuTru { get; set; } = string.Empty;
        public string DiaChiCoSo { get; set; } = string.Empty;
        public string PhuongXa { get; set; } = string.Empty;
        public string NguoiDaiDien { get; set; } = string.Empty;
        public string SoDienThoaiCoSo { get; set; } = string.Empty;
        public bool CoSoDaDuocDuyet { get; set; } = true;

        // ===== Thông tin người nước ngoài =====
        [Required(ErrorMessage = "Họ và tên khách không được để trống")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Quốc tịch không được để trống")]
        [Display(Name = "Quốc tịch")]
        public string QuocTich { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại khách (nếu có)")]
        public string? SoDienThoaiKhach { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email khách (nếu có)")]
        public string? EmailKhach { get; set; }

        // ===== Hộ chiếu =====
        [Required(ErrorMessage = "Số hộ chiếu không được để trống")]
        [Display(Name = "Số hộ chiếu")]
        public string SoHoChieu { get; set; } = string.Empty;

        [Display(Name = "Loại hộ chiếu")]
        public string? LoaiHoChieu { get; set; } = "Phổ thông";

        [DataType(DataType.Date)]
        [Display(Name = "Ngày cấp hộ chiếu")]
        public DateTime? NgayCapHoChieu { get; set; }

        [Required(ErrorMessage = "Hạn hộ chiếu không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn hộ chiếu")]
        public DateTime? NgayHetHanHoChieu { get; set; }

        [Display(Name = "Quốc gia cấp hộ chiếu")]
        public string? QuocGiaCapHoChieu { get; set; }

        // ===== Nhập cảnh / cư trú =====
        [Required(ErrorMessage = "Vui lòng chọn loại giấy tờ cư trú")]
        [Display(Name = "Loại giấy tờ cư trú")]
        public string LoaiGiayToCuTru { get; set; } = "Visa";

        [Display(Name = "Số visa / số giấy tờ cư trú")]
        public string? SoGiayToCuTru { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhập cảnh")]
        public DateTime? NgayNhapCanh { get; set; }

        [Display(Name = "Cửa khẩu nhập cảnh")]
        public string? CuaKhauNhapCanh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mục đích lưu trú")]
        [Display(Name = "Mục đích lưu trú")]
        public string MucDichLuuTru { get; set; } = "Du lịch";

        [Display(Name = "Mô tả mục đích khác")]
        public string? MucDichKhac { get; set; }

        // ===== Lưu trú tại cơ sở =====
        [Required(ErrorMessage = "Số phòng không được để trống")]
        [Display(Name = "Số phòng / Căn hộ")]
        public string SoPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu lưu trú không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu lưu trú")]
        public DateTime NgayBatDau { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc dự kiến")]
        public DateTime? NgayKetThucDuKien { get; set; }

        [Display(Name = "Ghi chú thêm (nếu có)")]
        public string? GhiChu { get; set; }

        // ===== Xác nhận =====
        [Required(ErrorMessage = "Vui lòng xác nhận cam kết trước khi gửi")]
        [MustBeTrueLocal(ErrorMessage = "Bạn phải cam kết thông tin khai báo là đúng sự thật")]
        [Display(Name = "Cam kết thông tin")]
        public bool CamKetThongTin { get; set; }
    }

    public class MustBeTrueLocalAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }
    }
}
