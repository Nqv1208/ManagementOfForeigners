using System.Collections.Generic;
using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Models.ViewModels.CongAn;

public class TongQuanViewModel
{
    public string TenDiaBan { get; set; } = string.Empty;
    public int TotalPending { get; set; }
    public int TotalApproved { get; set; }
    public int TotalForeigners { get; set; }
    public int TotalWarnings { get; set; }

    public List<HoSoKhaiBaoTamTru> RecentPending { get; set; } = new();
    public List<CanhBaoViPham> RecentWarnings { get; set; } = new();
}
