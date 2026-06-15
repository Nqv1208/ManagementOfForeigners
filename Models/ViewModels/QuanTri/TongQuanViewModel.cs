using System;
using System.Collections.Generic;

namespace ManagementOfForeigners.Models.ViewModels.QuanTri;

public class TongQuanViewModel
{
    public int TotalAccounts { get; set; }
    public int ActiveAccounts { get; set; }
    public int LockedAccounts { get; set; }
    public int NewAccountsThisMonth { get; set; }

    public int TotalForeigners { get; set; }
    public int VisaExpiring30Days { get; set; }
    public int TotalFacilities { get; set; }
    public int ActiveFacilities { get; set; }

    public int PendingDeclarations { get; set; }
    public int ApprovedDeclarations { get; set; }
    public int RejectedDeclarations { get; set; }

    public int TotalWarnings { get; set; }
    public int OpenReports { get; set; }

    public List<AdminStatItemViewModel> RoleStats { get; set; } = new();
    public List<AdminStatItemViewModel> DeclarationStats { get; set; } = new();
    public List<AdminAccountRowViewModel> LatestAccounts { get; set; } = new();
    public List<AdminDeclarationRowViewModel> LatestDeclarations { get; set; } = new();
    public List<AdminReportRowViewModel> LatestReports { get; set; } = new();
}

public class AdminStatItemViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class AdminAccountRowViewModel
{
    public string MaTaiKhoan { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public DateTime? LanDangNhapCuoi { get; set; }
    public string OwnerDisplayName { get; set; } = "—";
}

public class AdminFacilityRowViewModel
{
    public string MaCoSoLuuTru { get; set; } = string.Empty;
    public string TenCoSo { get; set; } = string.Empty;
    public string DiaChi = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string ChuCoSo { get; set; } = "—";
    public int SoKhachDangO { get; set; }
    public int TongHoSoKhaiBao { get; set; }
}

public class AdminDeclarationRowViewModel
{
    public string MaHSKhaiBao { get; set; } = string.Empty;
    public string HoTenNguoiNuocNgoai { get; set; } = "—";
    public string SoHoChieu { get; set; } = "—";
    public string DiaChiLuuTru { get; set; } = string.Empty;
    public string? TenCoSo { get; set; }
    public DateTime NgayKhaiBao { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

public class AdminReportRowViewModel
{
    public string MaBaoCao { get; set; } = string.Empty;
    public string HoTenNguoiNuocNgoai { get; set; } = "—";
    public string SoHoChieu { get; set; } = "—";
    public string NoiDungBaoCao { get; set; } = string.Empty;
    public DateTime NgayBaoCao { get; set; }
    public string CanBoBaoCao { get; set; } = "—";
    public string TrangThaiXuLy { get; set; } = string.Empty;
}
