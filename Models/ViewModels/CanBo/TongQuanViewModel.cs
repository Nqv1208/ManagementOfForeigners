using System.Collections.Generic;
using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Models.ViewModels.CanBo;

public class TongQuanViewModel
{
    public int TotalForeigners { get; set; }
    public int TotalDeclarations { get; set; }
    public int TotalWarnings { get; set; }
    public int TotalReports { get; set; }

    public List<HoSoKhaiBaoTamTru> RecentDeclarations { get; set; } = new();
    public List<CanhBaoViPham> RecentWarnings { get; set; } = new();
    public List<BaoCaoViPham> RecentReports { get; set; } = new();
}
