using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementOfForeigners.Models.ViewModels.Foreigner;

public class KhaiBaoTamTruCreateViewModel
{
    // Thông tin người nước ngoài read-only
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string QuocTich { get; set; } = string.Empty;
    public string SoHoChieu { get; set; } = string.Empty;
    public string? LoaiHoChieu { get; set; }
    public DateTime? NgayHetHanHoChieu { get; set; }
    public string? SoVisa { get; set; }
    public string? LoaiVisa { get; set; }
    public DateTime? NgayHetHanVisa { get; set; }
    public DateTime? NgayNhapCanh { get; set; }
    public string? CuaKhauNhapCanh { get; set; }
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;

    // Thông tin khai báo
    [Required(ErrorMessage = "Vui lòng chọn Phường/Xã lưu trú")]
    [Display(Name = "Phường/Xã lưu trú")]
    public int? PhuongXaId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn cơ sở lưu trú hợp lệ từ danh sách gợi ý")]
    [Display(Name = "Cơ sở lưu trú")]
    public string CoSoLuuTruId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ lưu trú cụ thể")]
    [StringLength(255, ErrorMessage = "Địa chỉ cụ thể không vượt quá 255 ký tự")]
    [Display(Name = "Địa chỉ lưu trú cụ thể")]
    public string DiaChiLuuTruCuThe { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Số phòng / căn hộ không vượt quá 50 ký tự")]
    [Display(Name = "Số phòng / căn hộ")]
    public string? SoPhong { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu tạm trú")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu tạm trú")]
    public DateTime NgayBatDau { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc tạm trú")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc tạm trú")]
    public DateTime NgayKetThuc { get; set; } = DateTime.Today.AddMonths(1);

    [Required(ErrorMessage = "Vui lòng chọn mục đích lưu trú")]
    [Display(Name = "Mục đích lưu trú")]
    public string MucDichLuuTru { get; set; } = string.Empty;

    [Display(Name = "Mục đích cụ thể khác")]
    public string? MucDichKhac { get; set; }

    [StringLength(255, ErrorMessage = "Ghi chú thêm không vượt quá 255 ký tự")]
    [Display(Name = "Ghi chú thêm")]
    public string? GhiChu { get; set; }

    [Display(Name = "Tôi cam kết các thông tin khai báo trên là đúng sự thật")]
    public bool CamKetThongTin { get; set; }

    // Dropdown data
    public List<SelectListItem> PhuongXaOptions { get; set; } = new List<SelectListItem>();
}
