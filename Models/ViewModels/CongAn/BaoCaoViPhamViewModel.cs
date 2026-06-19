using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.CongAn;

public class BaoCaoViPhamViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn người nước ngoài")]
    [Display(Name = "Người nước ngoài")]
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung báo cáo không được để trống")]
    [StringLength(255, ErrorMessage = "Nội dung báo cáo tối đa 255 ký tự")]
    [Display(Name = "Nội dung vi phạm / Lý do báo cáo")]
    public string NoiDungBaoCao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại báo cáo")]
    [Display(Name = "Loại báo cáo")]
    public string LoaiBaoCao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn mức độ khẩn cấp")]
    [Display(Name = "Mức độ khẩn cấp")]
    public string MucDoKhanCap { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Đề xuất xử lý tối đa 500 ký tự")]
    [Display(Name = "Đề xuất xử lý")]
    public string DeXuatXuLy { get; set; } = string.Empty;
}
