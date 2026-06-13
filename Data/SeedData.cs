using ManagementOfForeigners.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfForeigners.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await SeedPhuongXaAsync(context);
        await SeedVaiTroAsync(context);
        await SeedQuyenHanAsync(context);
        await SeedTaiKhoanAsync(context);
        await SeedCanBoAsync(context);
        await SeedChuCoSoAsync(context);
        await SeedCoSoAsync(context);
        await SeedNguoiNuocNgoaiAsync(context);
        await SeedHoSoTamTruAsync(context);
        await SeedLichSuCuTruAsync(context);
        await SeedViolationDataAsync(context);
    }

    private static async Task SeedPhuongXaAsync(ApplicationDbContext context)
    {
        if (await context.PhuongXas.AnyAsync())
        {
            return;
        }

        context.PhuongXas.AddRange(
            new PhuongXa { MaPhuongXa = 1, TenPhuongXa = "Hòa Cường Bắc" },
            new PhuongXa { MaPhuongXa = 2, TenPhuongXa = "Thạch Thang" },
            new PhuongXa { MaPhuongXa = 3, TenPhuongXa = "An Hải" },
            new PhuongXa { MaPhuongXa = 4, TenPhuongXa = "Hòa Minh" },
            new PhuongXa { MaPhuongXa = 5, TenPhuongXa = "Thanh Khê Đông" });

        await context.SaveChangesAsync();
    }

    private static async Task SeedVaiTroAsync(ApplicationDbContext context)
    {
        if (await context.VaiTros.AnyAsync())
        {
            return;
        }

        context.VaiTros.AddRange(
            new VaiTro { MaVaiTro = 1, TenVaiTro = VaiTroConst.NguoiNuocNgoai, MoTaVaiTro = "Người nước ngoài", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 2, TenVaiTro = VaiTroConst.ChuLuuTru, MoTaVaiTro = "Chủ cơ sở lưu trú", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 3, TenVaiTro = VaiTroConst.CongAn, MoTaVaiTro = "Công an Phường/Xã", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 4, TenVaiTro = VaiTroConst.CanBoXNC, MoTaVaiTro = "Cán bộ quản lý xuất nhập cảnh", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 5, TenVaiTro = VaiTroConst.Admin, MoTaVaiTro = "Quản trị viên hệ thống", TrangThai = "Hoạt động" });

        await context.SaveChangesAsync();
    }

    private static async Task SeedQuyenHanAsync(ApplicationDbContext context)
    {
        if (await context.QuyenHans.AnyAsync())
        {
            return;
        }

        context.QuyenHans.AddRange(
            Permission(1, "Foreigner.Profile", "Tra cứu và cập nhật thông tin cá nhân"),
            Permission(1, "Foreigner.ResidenceDeclaration", "Khai báo tạm trú"),
            Permission(2, "Accommodation.StayDeclaration", "Khai báo và cập nhật trạng thái lưu trú"),
            Permission(2, "Accommodation.FacilityProfile", "Cập nhật thông tin cơ sở lưu trú"),
            Permission(3, "Police.ApproveDeclaration", "Phê duyệt hoặc từ chối khai báo tạm trú theo địa bàn"),
            Permission(3, "Police.ViolationReport", "Cảnh báo và báo cáo vi phạm theo địa bàn"),
            Permission(4, "Officer.SearchForeigner", "Tra cứu người nước ngoài toàn thành phố"),
            Permission(4, "Officer.ReportStatistics", "Thống kê, lập báo cáo và xử lý báo cáo vi phạm"),
            Permission(5, "Admin.AccountRole", "Quản lý tài khoản, vai trò, quyền và phường xã"));

        await context.SaveChangesAsync();

        static QuyenHan Permission(int roleId, string name, string description)
        {
            return new QuyenHan
            {
                MaVaiTro = roleId,
                TenQuyen = name,
                MoTaQuyen = description,
                NgayCapNhat = DateTime.Now
            };
        }
    }

    private static async Task SeedTaiKhoanAsync(ApplicationDbContext context)
    {
        if (await context.TaiKhoans.AnyAsync())
        {
            return;
        }

        context.TaiKhoans.AddRange(
            Account("TK000001", "admin", "Admin@123", 5, "admin@danang.gov.vn", "0236000001"),
            Account("TK000002", "canbo01", "CanBo@123", 4, "canbo01@danang.gov.vn", "0236000002"),
            Account("TK000003", "congan01", "CongAn@123", 3, "congan01@danang.gov.vn", "0236000003"),
            Account("TK000004", "congan02", "CongAn@123", 3, "congan02@danang.gov.vn", "0236000004"),
            Account("TK000005", "khachsan01", "KhachSan@123", 2, "khachsan01@danang.vn", "0236100001"),
            Account("TK000006", "nhatro01", "NhaTro@123", 2, "nhatro01@danang.vn", "0236100002"),
            Account("TK000007", "john.smith", "User@123", 1, "john.smith@example.com", "0901234567"),
            Account("TK000008", "yuki.tanaka", "User@123", 1, "yuki.tanaka@example.com", "0901234568"),
            Account("TK000009", "park.minjun", "User@123", 1, "park.minjun@example.com", "0901234569"));

        await context.SaveChangesAsync();

        static TaiKhoan Account(string id, string username, string password, int roleId, string email, string phone)
        {
            return new TaiKhoan
            {
                MaTaiKhoan = id,
                TenDangNhap = username,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(password),
                MaVaiTro = roleId,
                Email = email,
                SoDienThoai = phone,
                TrangThai = TrangThaiTaiKhoan.HoatDong,
                NgayTao = DateTime.Now
            };
        }
    }

    private static async Task SeedCanBoAsync(ApplicationDbContext context)
    {
        if (await context.CanBos.AnyAsync())
        {
            return;
        }

        context.CanBos.AddRange(
            new CanBo
            {
                MaCanBo = "CB000001",
                MaPhuongXa = 1,
                MaTaiKhoan = "TK000003",
                HoTen = "Nguyễn Văn Hùng",
                SoCCCD = "048090000123",
                NgayCapCCCD = new DateTime(2018, 5, 20),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = "12 Trần Hưng Đạo, Hải Châu, Đà Nẵng",
                NgaySinh = new DateTime(1980, 10, 12),
                GioiTinh = "Nam",
                DonViCongTac = "Công an Phường Hòa Cường Bắc",
                ChucVu = "Cán bộ quản lý cư trú",
                CapQuanLy = "Phường",
                TrangThai = "Hoạt động"
            },
            new CanBo
            {
                MaCanBo = "CB000002",
                MaPhuongXa = 3,
                MaTaiKhoan = "TK000004",
                HoTen = "Trần Thị Lan",
                SoCCCD = "048090000456",
                NgayCapCCCD = new DateTime(2019, 3, 15),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = "45 Lê Lợi, Hải Châu, Đà Nẵng",
                NgaySinh = new DateTime(1985, 8, 24),
                GioiTinh = "Nữ",
                DonViCongTac = "Công an Phường An Hải",
                ChucVu = "Cán bộ quản lý cư trú",
                CapQuanLy = "Phường",
                TrangThai = "Hoạt động"
            },
            new CanBo
            {
                MaCanBo = "CB000003",
                MaPhuongXa = 2,
                MaTaiKhoan = "TK000002",
                HoTen = "Lê Hải Sơn",
                SoCCCD = "048090000789",
                NgayCapCCCD = new DateTime(2017, 9, 8),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = "78 Nguyễn Chí Thanh, Hải Châu, Đà Nẵng",
                NgaySinh = new DateTime(1978, 12, 5),
                GioiTinh = "Nam",
                DonViCongTac = "Phòng Quản lý Xuất nhập cảnh TP Đà Nẵng",
                ChucVu = "Cán bộ chuyên trách",
                CapQuanLy = "Thành phố",
                TrangThai = "Hoạt động"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedChuCoSoAsync(ApplicationDbContext context)
    {
        if (await context.ChuCoSoLuuTrus.AnyAsync())
        {
            return;
        }

        context.ChuCoSoLuuTrus.AddRange(
            new ChuCoSoLuuTru
            {
                MaChuCoSo = "CC000001",
                MaTaiKhoan = "TK000005",
                HoTen = "Phạm Quang Minh",
                NgaySinh = new DateTime(1975, 4, 3),
                GioiTinh = "Nam",
                SoCCCD = "048090001111",
                NgayCapCCCD = new DateTime(2016, 12, 12),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = "89 Hùng Vương, Hải Châu, Đà Nẵng"
            },
            new ChuCoSoLuuTru
            {
                MaChuCoSo = "CC000002",
                MaTaiKhoan = "TK000006",
                HoTen = "Nguyễn Thị Bình",
                NgaySinh = new DateTime(1982, 11, 20),
                GioiTinh = "Nữ",
                SoCCCD = "048090002222",
                NgayCapCCCD = new DateTime(2018, 7, 19),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = "120 Điện Biên Phủ, Thanh Khê, Đà Nẵng"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedCoSoAsync(ApplicationDbContext context)
    {
        if (await context.CoSoLuuTrus.AnyAsync())
        {
            return;
        }

        context.CoSoLuuTrus.AddRange(
            new CoSoLuuTru
            {
                MaCoSoLuuTru = "CS000001",
                MaPhuongXa = 1,
                MaChuCoSo = "CC000001",
                TenCoSo = "Khách sạn Sông Hàn Đà Nẵng",
                DiaChi = "36 Bạch Đằng, Phường Hòa Cường Bắc, Quận Hải Châu, Đà Nẵng",
                SoDienThoai = "0236382999",
                Email = "info@songhandanang.vn",
                TrangThai = TrangThaiCoSo.DangHoatDong
            },
            new CoSoLuuTru
            {
                MaCoSoLuuTru = "CS000002",
                MaPhuongXa = 5,
                MaChuCoSo = "CC000002",
                TenCoSo = "Nhà trọ An Bình",
                DiaChi = "125 Nguyễn Văn Linh, Phường Thanh Khê Đông, Quận Thanh Khê, Đà Nẵng",
                SoDienThoai = "0236350123",
                Email = "anbinh@danang.vn",
                TrangThai = TrangThaiCoSo.DangHoatDong
            },
            new CoSoLuuTru
            {
                MaCoSoLuuTru = "CS000003",
                MaPhuongXa = 3,
                MaChuCoSo = "CC000001",
                TenCoSo = "Căn hộ biển An Hải",
                DiaChi = "18 Võ Văn Kiệt, Phường An Hải, Quận Sơn Trà, Đà Nẵng",
                SoDienThoai = "0236366888",
                Email = "anhai.apartment@danang.vn",
                TrangThai = TrangThaiCoSo.DangHoatDong
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedNguoiNuocNgoaiAsync(ApplicationDbContext context)
    {
        if (await context.NguoiNuocNgoais.AnyAsync())
        {
            return;
        }

        context.NguoiNuocNgoais.AddRange(
            Foreigner("NN000001", "TK000007", "John Smith", new DateTime(1990, 5, 15), "Nam", "Mỹ", "US12345678", "Du lịch", new DateTime(2026, 12, 31)),
            Foreigner("NN000002", "TK000008", "Yuki Tanaka", new DateTime(1988, 3, 22), "Nữ", "Nhật Bản", "JP98765432", "Lao động", new DateTime(2027, 6, 30)),
            Foreigner("NN000003", "TK000009", "Park Min-jun", new DateTime(1995, 11, 8), "Nam", "Hàn Quốc", "KR55667788", "Học tập", new DateTime(2027, 8, 31)));

        await context.SaveChangesAsync();

        static NguoiNuocNgoai Foreigner(string id, string accountId, string name, DateTime birthday, string gender, string nationality, string passport, string visaType, DateTime visaExpiry)
        {
            return new NguoiNuocNgoai
            {
                MaNguoiNuocNgoai = id,
                MaTaiKhoan = accountId,
                HoTen = name,
                NgaySinh = birthday,
                GioiTinh = gender,
                QuocTich = nationality,
                SoHoChieu = passport,
                NgayCapHoChieu = new DateTime(2021, 1, 10),
                NgayHetHanHoChieu = new DateTime(2031, 1, 10),
                LoaiVisa = visaType,
                NgayHetHanVisa = visaExpiry
            };
        }
    }

    private static async Task SeedHoSoTamTruAsync(ApplicationDbContext context)
    {
        if (await context.HoSoKhaiBaoTamTrus.AnyAsync())
        {
            return;
        }

        context.HoSoKhaiBaoTamTrus.AddRange(
            new HoSoKhaiBaoTamTru
            {
                MaHSKhaiBao = "HS000001",
                MaTaiKhoan = "TK000007",
                MaCoSoLuuTru = "CS000001",
                NgayKhaiBao = DateTime.Today.AddDays(-10),
                NgayBatDau = DateTime.Today.AddDays(-10),
                NgayKetThuc = DateTime.Today.AddDays(20),
                MucDichLuuTru = "Du lịch",
                DiaChiLuuTru = "36 Bạch Đằng, Phường Hòa Cường Bắc, Quận Hải Châu, Đà Nẵng",
                TrangThai = TrangThaiKhaiBao.DaDuyet,
                GhiChu = "Khách du lịch ngắn hạn"
            },
            new HoSoKhaiBaoTamTru
            {
                MaHSKhaiBao = "HS000002",
                MaTaiKhoan = "TK000008",
                MaCoSoLuuTru = "CS000003",
                NgayKhaiBao = DateTime.Today.AddDays(-5),
                NgayBatDau = DateTime.Today.AddDays(-5),
                NgayKetThuc = DateTime.Today.AddMonths(6),
                MucDichLuuTru = "Công tác",
                DiaChiLuuTru = "18 Võ Văn Kiệt, Phường An Hải, Quận Sơn Trà, Đà Nẵng",
                TrangThai = TrangThaiKhaiBao.ChoDuyet,
                GhiChu = "Làm việc tại công ty CNTT"
            },
            new HoSoKhaiBaoTamTru
            {
                MaHSKhaiBao = "HS000003",
                MaTaiKhoan = "TK000009",
                MaCoSoLuuTru = "CS000002",
                NgayKhaiBao = DateTime.Today.AddDays(-2),
                NgayBatDau = DateTime.Today.AddDays(-2),
                NgayKetThuc = DateTime.Today.AddMonths(12),
                MucDichLuuTru = "Học tập",
                DiaChiLuuTru = "125 Nguyễn Văn Linh, Phường Thanh Khê Đông, Quận Thanh Khê, Đà Nẵng",
                TrangThai = TrangThaiKhaiBao.ChoDuyet,
                GhiChu = "Sinh viên trao đổi"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedLichSuCuTruAsync(ApplicationDbContext context)
    {
        if (await context.LichSuCuTrus.AnyAsync())
        {
            return;
        }

        context.LichSuCuTrus.AddRange(
            new LichSuCuTru
            {
                MaLSLuuTru = "LS000001",
                MaNguoiNuocNgoai = "NN000001",
                MaCoSoLuuTru = "CS000001",
                NgayBatDau = DateTime.Now.AddDays(-10),
                Phong = "501",
                TrangThai = TrangThaiLuuTru.DangO,
                GhiChu = "Khách du lịch ngắn hạn"
            },
            new LichSuCuTru
            {
                MaLSLuuTru = "LS000002",
                MaNguoiNuocNgoai = "NN000002",
                MaCoSoLuuTru = "CS000003",
                NgayBatDau = DateTime.Now.AddDays(-5),
                Phong = "A-1202",
                TrangThai = TrangThaiLuuTru.DangO,
                GhiChu = "Khách công tác"
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedViolationDataAsync(ApplicationDbContext context)
    {
        if (!await context.CanhBaoViPhams.AnyAsync())
        {
            context.CanhBaoViPhams.Add(new CanhBaoViPham
            {
                MaNguoiNuocNgoai = "NN000002",
                MaCanBo = "CB000003",
                LoaiViPham = "Nhắc hạn khai báo",
                NoiDungCanhBao = "Yêu cầu bổ sung thông tin lưu trú trong thời hạn quy định.",
                MucDoViPham = MucDoViPhamConst.Nhe,
                TrangThai = TrangThaiCanhBao.DaGui,
                NgayCanhBao = DateTime.Now.AddDays(-1)
            });
        }

        if (!await context.BaoCaoViPhams.AnyAsync())
        {
            context.BaoCaoViPhams.Add(new BaoCaoViPham
            {
                MaBaoCao = "BC000001",
                MaNguoiNuocNgoai = "NN000002",
                MaCanBo = "CB000002",
                NoiDungBaoCao = "Báo cáo hồ sơ cần cán bộ xuất nhập cảnh rà soát thêm.",
                NgayBaoCao = DateTime.Now.AddHours(-8),
                TrangThaiXuLy = TrangThaiXuLyConst.ChuaXuLy
            });
        }

        await context.SaveChangesAsync();
    }
}
