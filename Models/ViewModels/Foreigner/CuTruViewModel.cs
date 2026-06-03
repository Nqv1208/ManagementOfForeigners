using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.Foreigner;

public class CuTruViewModel
{
    [Required(ErrorMessage = "Phòng/Căn hộ không được để trống")]
    [StringLength(20)]
    [Display(Name = "Số phòng / Số căn hộ")]
    public string Phong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cơ sở lưu trú không được để trống")]
    [Display(Name = "Chọn cơ sở lưu trú")]
    public string MaCoSoLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu lưu trú không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; } = DateTime.Now;

    [StringLength(255)]
    [Display(Name = "Ghi chú thêm")]
    public string? GhiChu { get; set; }
}
