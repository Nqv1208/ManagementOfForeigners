using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Models.ViewModels.Accommodation;

public class DashboardViewModel
{
    public CoSoLuuTru CoSoInfo { get; set; } = null!;
    public int TotalStaying { get; set; }
    public int TotalStayed { get; set; }
    public int TotalOverdue { get; set; }
    public List<LichSuCuTru> ActiveStays { get; set; } = new();
    public List<LichSuCuTru> RecentCheckins { get; set; } = new();
}
