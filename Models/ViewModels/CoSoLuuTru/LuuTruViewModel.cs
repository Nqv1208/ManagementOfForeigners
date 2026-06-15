using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.CoSoLuuTru;

public class LuuTruViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn người nước ngoài")]
    [Display(Name = "Chọn người nước ngoài")]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số phòng không được để trống")]
    [StringLength(20)]
    [Display(Name = "Số phòng")]
    public string Phong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu lưu trú không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc dự kiến")]
    public DateTime? NgayKetThuc { get; set; }

    [StringLength(255)]
    [Display(Name = "Ghi chú thêm")]
    public string? GhiChu { get; set; }
}
