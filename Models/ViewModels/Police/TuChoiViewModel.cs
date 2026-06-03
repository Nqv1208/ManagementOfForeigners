using System.ComponentModel.DataAnnotations;

namespace ManagementOfForeigners.Models.ViewModels.Police;

public class TuChoiViewModel
{
    [Required]
    public string MaHSKhaiBao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lý do từ chối không được để trống")]
    [StringLength(225, ErrorMessage = "Lý do từ chối tối đa 225 ký tự")]
    [Display(Name = "Lý do từ chối")]
    public string LyDoTuChoi { get; set; } = string.Empty;
}
