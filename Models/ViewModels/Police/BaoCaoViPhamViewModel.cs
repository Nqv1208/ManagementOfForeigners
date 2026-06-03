using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.Police;

public class BaoCaoViPhamViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn người nước ngoài")]
    [Display(Name = "Người nước ngoài")]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung báo cáo không được để trống")]
    [StringLength(255, ErrorMessage = "Nội dung báo cáo tối đa 255 ký tự")]
    [Display(Name = "Nội dung vi phạm / Lý do báo cáo")]
    public string NoiDungBaoCao { get; set; } = string.Empty;
}
