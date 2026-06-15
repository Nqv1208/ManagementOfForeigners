namespace ManagementOfForeigners.Models.ViewModels.QuanTri;

public class NhatKyHoatDongViewModel
{
    public DateTime OccurredAt { get; set; }
    public string ActorOrSource { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
