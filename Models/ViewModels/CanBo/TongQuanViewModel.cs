using System.Collections.Generic;
using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Models.ViewModels.CanBo;

public class TongQuanViewModel
{
    public int TotalForeigners { get; set; }
    public int TotalDeclarations { get; set; }
    public int TotalWarnings { get; set; }
    public int TotalReports { get; set; }

    // New Operational metrics
    public int ActiveStaysCount { get; set; }
    public int NewDeclarationsCount { get; set; }
    public int ReviewNeededCount { get; set; }
    public int PendingReportsCount { get; set; }
    public int RiskyFacilitiesCount { get; set; }

    public List<HoSoKhaiBaoTamTru> RecentDeclarations { get; set; } = new();
    public List<CanhBaoViPham> RecentWarnings { get; set; } = new();
    public List<BaoCaoViPham> RecentReports { get; set; } = new();

    // New lists for dashboard panels
    public List<HoSoKhaiBaoTamTru> ReviewNeededDeclarations { get; set; } = new();
    public List<BaoCaoViPham> PendingReports { get; set; } = new();
    public List<CanBoFacilityRowViewModel> RiskyFacilities { get; set; } = new();
}

public class CanBoFacilityRowViewModel
{
    public string MaCoSoLuuTru { get; set; } = string.Empty;
    public string TenCoSo { get; set; } = string.Empty;
    public string LoaiHinh { get; set; } = string.Empty;
    public string PhuongXa { get; set; } = string.Empty;
    public int SoKhachDangO { get; set; }
    public int TongHoSo { get; set; }
    public int TongViPham { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string MucRuiRo { get; set; } = "Thấp";
}
