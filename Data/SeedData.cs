using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Data;

/// <summary>
/// Seed dữ liệu mẫu cho hệ thống
/// </summary>
public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Đảm bảo database đã được tạo
        context.Database.EnsureCreated();

        // 1. Seed bảng VaiTro
        if (!context.VaiTros.Any())
        {
            var vaiTros = new List<VaiTro>
            {
                new VaiTro { MaVaiTro = 1, TenVaiTro = VaiTroConst.NguoiNuocNgoai, MoTaVaiTro = "Người nước ngoại", TrangThai = "Hoạt động", NgayTao = DateTime.Now },
                new VaiTro { MaVaiTro = 2, TenVaiTro = VaiTroConst.ChuLuuTru, MoTaVaiTro = "Chủ cơ sở lưu trú", TrangThai = "Hoạt động", NgayTao = DateTime.Now },
                new VaiTro { MaVaiTro = 3, TenVaiTro = VaiTroConst.CongAn, MoTaVaiTro = "Công an Phường/Xã", TrangThai = "Hoạt động", NgayTao = DateTime.Now },
                new VaiTro { MaVaiTro = 4, TenVaiTro = VaiTroConst.CanBoXNC, MoTaVaiTro = "Cán bộ quản lý xuất nhập cảnh", TrangThai = "Hoạt động", NgayTao = DateTime.Now },
                new VaiTro { MaVaiTro = 5, TenVaiTro = VaiTroConst.Admin, MoTaVaiTro = "Quản trị viên hệ thống", TrangThai = "Hoạt động", NgayTao = DateTime.Now }
            };
            context.VaiTros.AddRange(vaiTros);
            context.SaveChanges();
        }

        // Nếu đã có tài khoản thì không seed tiếp các bảng khác
        if (context.TaiKhoans.Any())
            return;

        // ===== Tài khoản Admin =====
        var admin = new TaiKhoan
        {
            MaTaiKhoan = "TK000001",
            TenDangNhap = "admin",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            MaVaiTro = 5,
            Email = "admin@danang.gov.vn",
            SoDienThoai = "0236000001",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        // ===== Tài khoản Cán bộ XNC =====
        var canBo1 = new TaiKhoan
        {
            MaTaiKhoan = "TK000002",
            TenDangNhap = "canbo01",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("CanBo@123"),
            MaVaiTro = 4,
            Email = "canbo01@danang.gov.vn",
            SoDienThoai = "0236000002",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        // ===== Tài khoản Công an =====
        var congAn1 = new TaiKhoan
        {
            MaTaiKhoan = "TK000003",
            TenDangNhap = "congan01",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("CongAn@123"),
            MaVaiTro = 3,
            Email = "congan01@danang.gov.vn",
            SoDienThoai = "0236000003",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        var congAn2 = new TaiKhoan
        {
            MaTaiKhoan = "TK000004",
            TenDangNhap = "congan02",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("CongAn@123"),
            MaVaiTro = 3,
            Email = "congan02@danang.gov.vn",
            SoDienThoai = "0236000004",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        // ===== Tài khoản Chủ cơ sở lưu trú =====
        var chuLuuTru1 = new TaiKhoan
        {
            MaTaiKhoan = "TK000005",
            TenDangNhap = "khachsan01",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("KhachSan@123"),
            MaVaiTro = 2,
            Email = "khachsan01@gmail.com",
            SoDienThoai = "0236100001",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        var chuLuuTru2 = new TaiKhoan
        {
            MaTaiKhoan = "TK000006",
            TenDangNhap = "nhatro01",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("NhaTro@123"),
            MaVaiTro = 2,
            Email = "nhatro01@gmail.com",
            SoDienThoai = "0236100002",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        // ===== Tài khoản Người nước ngoài =====
        var nnn1 = new TaiKhoan
        {
            MaTaiKhoan = "TK000007",
            TenDangNhap = "john.smith",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            MaVaiTro = 1,
            Email = "john.smith@gmail.com",
            SoDienThoai = "0901234567",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        var nnn2 = new TaiKhoan
        {
            MaTaiKhoan = "TK000008",
            TenDangNhap = "yuki.tanaka",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            MaVaiTro = 1,
            Email = "yuki.tanaka@gmail.com",
            SoDienThoai = "0901234568",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        var nnn3 = new TaiKhoan
        {
            MaTaiKhoan = "TK000009",
            TenDangNhap = "park.minjun",
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            MaVaiTro = 1,
            Email = "park.minjun@gmail.com",
            SoDienThoai = "0901234569",
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            NgayTao = DateTime.Now
        };

        context.TaiKhoans.AddRange(admin, canBo1, congAn1, congAn2, chuLuuTru1, chuLuuTru2, nnn1, nnn2, nnn3);
        context.SaveChanges();

        // ===== Cán bộ =====
        var cb1 = new CanBo
        {
            MaCanBo = "CB000001",
            MaTaiKhoan = "TK000003", // congan01
            HoTen = "Nguyễn Văn Hùng",
            SoCCCD = "048090000123",
            NgayCapCCCD = new DateTime(2018, 5, 20),
            NoiCapCCCD = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
            DiaChiThuongTru = "12 Trần Hưng Đạo, Hải Châu, Đà Nẵng",
            NgaySinh = new DateTime(1980, 10, 12),
            GioiTinh = "Nam",
            DonViCongTac = "Công an Phường Hòa Cường Bắc",
            ChucVu = "Trưởng Công an Phường",
            CapQuanLy = "Phường",
            TrangThai = "Hoạt động"
        };

        var cb2 = new CanBo
        {
            MaCanBo = "CB000002",
            MaTaiKhoan = "TK000004", // congan02
            HoTen = "Trần Thị Lan",
            SoCCCD = "048090000456",
            NgayCapCCCD = new DateTime(2019, 3, 15),
            NoiCapCCCD = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
            DiaChiThuongTru = "45 Lê Lợi, Hải Châu, Đà Nẵng",
            NgaySinh = new DateTime(1985, 8, 24),
            GioiTinh = "Nữ",
            DonViCongTac = "Công an Phường Thạch Thang",
            ChucVu = "Cán bộ quản lý cư trú",
            CapQuanLy = "Phường",
            TrangThai = "Hoạt động"
        };

        var cb3 = new CanBo
        {
            MaCanBo = "CB000003",
            MaTaiKhoan = "TK000002", // canbo01
            HoTen = "Lê Hải Sơn",
            SoCCCD = "048090000789",
            NgayCapCCCD = new DateTime(2017, 9, 8),
            NoiCapCCCD = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
            DiaChiThuongTru = "78 Nguyễn Chí Thanh, Hải Châu, Đà Nẵng",
            NgaySinh = new DateTime(1978, 12, 5),
            GioiTinh = "Nam",
            DonViCongTac = "Phòng Quản lý Xuất Nhập Cảnh TP Đà Nẵng",
            ChucVu = "Cán bộ chuyên trách",
            CapQuanLy = "Tỉnh",
            TrangThai = "Hoạt động"
        };

        context.CanBos.AddRange(cb1, cb2, cb3);
        context.SaveChanges();

        // ===== Chủ cơ sở lưu trú =====
        var ccs1 = new ChuCoSoLuuTru
        {
            MaChuCoSo = "CC000001",
            MaTaiKhoan = "TK000005", // khachsan01
            HoTen = "Phạm Quang Minh",
            NgaySinh = new DateTime(1975, 4, 3),
            GioiTinh = "Nam",
            SoCCCD = "048090001111",
            NgayCapCCCD = new DateTime(2016, 12, 12),
            NoiCapCCCD = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
            DiaChiThuongTru = "89 Hùng Vương, Hải Châu, Đà Nẵng"
        };

        var ccs2 = new ChuCoSoLuuTru
        {
            MaChuCoSo = "CC000002",
            MaTaiKhoan = "TK000006", // nhatro01
            HoTen = "Nguyễn Thị Bình",
            NgaySinh = new DateTime(1982, 11, 20),
            GioiTinh = "Nữ",
            SoCCCD = "048090002222",
            NgayCapCCCD = new DateTime(2018, 7, 19),
            NoiCapCCCD = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư",
            DiaChiThuongTru = "120 Điện Biên Phủ, Thanh Khê, Đà Nẵng"
        };

        context.ChuCoSoLuuTrus.AddRange(ccs1, ccs2);
        context.SaveChanges();

        // ===== Cơ sở lưu trú =====
        var cs1 = new CoSoLuuTru
        {
            MaCoSoLuuTru = "CS000001",
            TenCoSo = "Khách sạn Novotel Đà Nẵng",
            DiaChi = "36 Bạch Đằng, Hải Châu, Đà Nẵng",
            SoDienThoai = "0236382999",
            Email = "info@novoteldanang.com",
            MaTaiKhoan = "TK000005",
            TrangThai = TrangThaiCoSo.DangHoatDong
        };

        var cs2 = new CoSoLuuTru
        {
            MaCoSoLuuTru = "CS000002",
            TenCoSo = "Nhà trọ An Bình",
            DiaChi = "125 Nguyễn Văn Linh, Thanh Khê, Đà Nẵng",
            SoDienThoai = "0236350123",
            Email = "anbinh.nhatro@gmail.com",
            MaTaiKhoan = "TK000006",
            TrangThai = TrangThaiCoSo.DangHoatDong
        };

        context.CoSoLuuTrus.AddRange(cs1, cs2);
        context.SaveChanges();

        // ===== Người nước ngoài =====
        var nguoi1 = new NguoiNuocNgoai
        {
            MaNguoiNuocNgoai = "NN000001",
            MaTaiKhoan = "TK000007",
            HoTen = "John Smith",
            NgaySinh = new DateTime(1990, 5, 15),
            GioiTinh = "Nam",
            QuocTich = "Mỹ",
            SoHoChieu = "US12345678",
            NgayCapHoChieu = new DateTime(2020, 1, 10),
            NgayHetHanHoChieu = new DateTime(2030, 1, 10),
            LoaiVisa = "Tourist",
            NgayHetHanVisa = new DateTime(2026, 12, 31)
        };

        var nguoi2 = new NguoiNuocNgoai
        {
            MaNguoiNuocNgoai = "NN000002",
            MaTaiKhoan = "TK000008",
            HoTen = "Yuki Tanaka",
            NgaySinh = new DateTime(1988, 3, 22),
            GioiTinh = "Nữ",
            QuocTich = "Nhật Bản",
            SoHoChieu = "JP98765432",
            NgayCapHoChieu = new DateTime(2021, 6, 1),
            NgayHetHanHoChieu = new DateTime(2031, 6, 1),
            LoaiVisa = "Work",
            NgayHetHanVisa = new DateTime(2027, 6, 30)
        };

        var nguoi3 = new NguoiNuocNgoai
        {
            MaNguoiNuocNgoai = "NN000003",
            MaTaiKhoan = "TK000009",
            HoTen = "Park Min-jun",
            NgaySinh = new DateTime(1995, 11, 8),
            GioiTinh = "Nam",
            QuocTich = "Hàn Quốc",
            SoHoChieu = "KR55667788",
            NgayCapHoChieu = new DateTime(2022, 2, 15),
            NgayHetHanHoChieu = new DateTime(2032, 2, 15),
            LoaiVisa = "Student",
            NgayHetHanVisa = new DateTime(2027, 8, 31)
        };

        context.NguoiNuocNgoais.AddRange(nguoi1, nguoi2, nguoi3);
        context.SaveChanges();

        // ===== Hồ sơ khai báo tạm trú mẫu =====
        var hs1 = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = "HS000001",
            MaTaiKhoan = "TK000007",
            NgayKhaiBao = DateTime.Now.AddDays(-10),
            NgayBatDau = DateTime.Now.AddDays(-10),
            NgayKetThuc = DateTime.Now.AddDays(20),
            MucDichLuuTru = "Du lịch",
            DiaChiLuuTru = "36 Bạch Đằng, Hải Châu, Đà Nẵng",
            MaCoSoLuuTru = "CS000001",
            TrangThai = TrangThaiKhaiBao.DaDuyet,
            GhiChu = "Khách du lịch ngắn hạn"
        };

        var hs2 = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = "HS000002",
            MaTaiKhoan = "TK000008",
            NgayKhaiBao = DateTime.Now.AddDays(-5),
            NgayBatDau = DateTime.Now.AddDays(-5),
            NgayKetThuc = DateTime.Now.AddMonths(6),
            MucDichLuuTru = "Công tác",
            DiaChiLuuTru = "125 Nguyễn Văn Linh, Thanh Khê, Đà Nẵng",
            MaCoSoLuuTru = "CS000002",
            TrangThai = TrangThaiKhaiBao.ChoDuyet,
            GhiChu = "Làm việc tại công ty CNTT"
        };

        var hs3 = new HoSoKhaiBaoTamTru
        {
            MaHSKhaiBao = "HS000003",
            MaTaiKhoan = "TK000009",
            NgayKhaiBao = DateTime.Now.AddDays(-2),
            NgayBatDau = DateTime.Now.AddDays(-2),
            NgayKetThuc = DateTime.Now.AddMonths(12),
            MucDichLuuTru = "Học tập",
            DiaChiLuuTru = "54 Nguyễn Lương Bằng, Liên Chiểu, Đà Nẵng",
            TrangThai = TrangThaiKhaiBao.ChoDuyet,
            GhiChu = "Sinh viên trao đổi"
        };

        context.HoSoKhaiBaoTamTrus.AddRange(hs1, hs2, hs3);
        context.SaveChanges();

        // ===== Lịch sử lưu trú mẫu =====
        var ls1 = new LichSuLuuTru
        {
            MaLSLuuTru = "LS000001",
            MaNguoiNuocNgoai = "NN000001",
            MaCoSoLuuTru = "CS000001",
            NgayBatDau = DateTime.Now.AddDays(-10),
            Phong = "501",
            TrangThai = TrangThaiLuuTru.DangO,
            GhiChu = "Khách VIP"
        };

        context.LichSuLuuTrus.Add(ls1);
        context.SaveChanges();
    }
}
