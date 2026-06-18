using System;
using X.PagedList;

namespace ManagementOfForeigners.Models.ViewModels.CanBo;

public class CanBoTraCuuViewModel
{
    public string? Search { get; set; }
    public string? QuocTich { get; set; }
    public string? LoaiVisa { get; set; }
    public string? TrangThaiCuTru { get; set; }
    public string? PhuongXa { get; set; }
    public string? CoSoLuuTru { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }

    public IPagedList<CanBoForeignerRowViewModel> Foreigners { get; set; } = null!;
}

public class CanBoForeignerRowViewModel
{
    public string MaNguoiNuocNgoai { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string SoHoChieu { get; set; } = string.Empty;
    public string QuocTich { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string LoaiVisa { get; set; } = string.Empty;
    public DateTime NgayHetHanVisa { get; set; }
    
    public string TenCoSoLuuTru { get; set; } = "—";
    public string TenPhuongXa { get; set; } = "—";
    public string DiaChiCuTru { get; set; } = "—";
    public string TrangThaiCuTru { get; set; } = "Ngoại tuyến";
}
