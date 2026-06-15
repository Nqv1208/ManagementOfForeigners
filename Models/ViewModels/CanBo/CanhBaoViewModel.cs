using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.CanBo;

public class CanhBaoViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn người nước ngoài")]
    [Display(Name = "Người nước ngoài")]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại vi phạm không được để trống")]
    [StringLength(100, ErrorMessage = "Loại vi phạm tối đa 100 ký tự")]
    [Display(Name = "Loại vi phạm")]
    public string LoaiViPham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung cảnh báo không được để trống")]
    [Display(Name = "Nội dung cảnh báo")]
    public string NoiDungCanhBao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mức độ vi phạm không được để trống")]
    [Display(Name = "Mức độ vi phạm")]
    public string MucDoViPham { get; set; } = string.Empty;

    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}
