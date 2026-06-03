using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.Foreigner;

public class KhaiBaoViewModel
{
    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddMonths(1);

    [Required(ErrorMessage = "Mục đích lưu trú không được để trống")]
    [StringLength(100, ErrorMessage = "Mục đích không dài quá 100 ký tự")]
    [Display(Name = "Mục đích lưu trú")]
    public string MucDichLuuTru { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ lưu trú không được để trống")]
    [StringLength(255, ErrorMessage = "Địa chỉ không dài quá 255 ký tự")]
    [Display(Name = "Địa chỉ lưu trú cụ thể")]
    public string DiaChiLuuTru { get; set; } = string.Empty;

    [Display(Name = "Cơ sở lưu trú (nếu có)")]
    public string? MaCoSoLuuTru { get; set; }

    [StringLength(225, ErrorMessage = "Ghi chú không dài quá 225 ký tự")]
    [Display(Name = "Ghi chú thêm")]
    public string? GhiChu { get; set; }
}
