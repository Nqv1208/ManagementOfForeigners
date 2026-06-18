using ManagementOfForeigners.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfForeigners.Data;

public static class SeedData
{
    private const int TargetPoliceWardCoverage = 94;
    private const int TargetOfficerCount = 5;
    private const int TargetOwnerCount = 120;
    private const int TargetFacilityCount = 180;
    private const int TargetForeignerCount = 400;
    private const int TargetDeclarationCount = 600;
    private const int TargetStayHistoryCount = 420;
    private const int TargetProfileUpdateCount = 150;
    private const int TargetWarningCount = 160;
    private const int TargetViolationReportCount = 100;

    private static readonly string[] DaNangAdministrativeUnitNames =
    [
        "Phường Hải Châu",
        "Phường Hòa Cường",
        "Phường Thanh Khê",
        "Phường An Khê",
        "Phường An Hải",
        "Phường Sơn Trà",
        "Phường Ngũ Hành Sơn",
        "Phường Hòa Khánh",
        "Phường Hải Vân",
        "Phường Liên Chiểu",
        "Phường Cẩm Lệ",
        "Phường Hòa Xuân",
        "Phường Tam Kỳ",
        "Phường Quảng Phú",
        "Phường Hương Trà",
        "Phường Bàn Thạch",
        "Phường Điện Bàn",
        "Phường Điện Bàn Đông",
        "Phường An Thắng",
        "Phường Điện Bàn Bắc",
        "Phường Hội An",
        "Phường Hội An Đông",
        "Phường Hội An Tây",
        "Xã Hòa Vang",
        "Xã Hòa Tiến",
        "Xã Bà Nà",
        "Xã Núi Thành",
        "Xã Tam Mỹ",
        "Xã Tam Anh",
        "Xã Đức Phú",
        "Xã Tam Xuân",
        "Xã Tây Hồ",
        "Xã Chiên Đàn",
        "Xã Phú Ninh",
        "Xã Lãnh Ngọc",
        "Xã Tiên Phước",
        "Xã Thạnh Bình",
        "Xã Sơn Cẩm Hà",
        "Xã Trà Liên",
        "Xã Trà Giáp",
        "Xã Trà Tân",
        "Xã Trà Đốc",
        "Xã Trà My",
        "Xã Nam Trà My",
        "Xã Trà Tập",
        "Xã Trà Vân",
        "Xã Trà Linh",
        "Xã Trà Leng",
        "Xã Thăng Bình",
        "Xã Thăng An",
        "Xã Thăng Trường",
        "Xã Thăng Điền",
        "Xã Thăng Phú",
        "Xã Đồng Dương",
        "Xã Quế Sơn Trung",
        "Xã Quế Sơn",
        "Xã Xuân Phú",
        "Xã Nông Sơn",
        "Xã Quế Phước",
        "Xã Duy Nghĩa",
        "Xã Nam Phước",
        "Xã Duy Xuyên",
        "Xã Thu Bồn",
        "Xã Điện Bàn Tây",
        "Xã Gò Nổi",
        "Xã Đại Lộc",
        "Xã Hà Nha",
        "Xã Thượng Đức",
        "Xã Vu Gia",
        "Xã Phú Thuận",
        "Xã Thạnh Mỹ",
        "Xã Bến Giằng",
        "Xã Nam Giang",
        "Xã Đắc Pring",
        "Xã La Dêê",
        "Xã La Êê",
        "Xã Sông Vàng",
        "Xã Sông Kôn",
        "Xã Đông Giang",
        "Xã Bến Hiên",
        "Xã Avương",
        "Xã Tây Giang",
        "Xã Hùng Sơn",
        "Xã Hiệp Đức",
        "Xã Việt An",
        "Xã Phước Trà",
        "Xã Khâm Đức",
        "Xã Phước Năng",
        "Xã Phước Chánh",
        "Xã Phước Thành",
        "Xã Phước Hiệp",
        "Đặc khu Hoàng Sa",
        "Xã Tam Hải",
        "Xã Tân Hiệp"
    ];

    private static readonly string[] StreetNames =
    [
        "Bạch Đằng",
        "Trần Phú",
        "Nguyễn Văn Linh",
        "Lê Duẩn",
        "Ngô Quyền",
        "Võ Nguyên Giáp",
        "Hoàng Diệu",
        "Điện Biên Phủ",
        "Hùng Vương",
        "Nguyễn Tất Thành",
        "Phan Châu Trinh",
        "Trưng Nữ Vương"
    ];

    private static readonly string[] VietnameseFamilyNames =
    [
        "Nguyễn",
        "Trần",
        "Lê",
        "Phạm",
        "Hoàng",
        "Võ",
        "Đặng",
        "Bùi",
        "Đỗ",
        "Hồ"
    ];

    private static readonly string[] VietnameseMiddleNames =
    [
        "Văn",
        "Thị",
        "Minh",
        "Hải",
        "Quang",
        "Thanh",
        "Hoài",
        "Anh",
        "Gia",
        "Khánh"
    ];

    private static readonly string[] VietnameseGivenNames =
    [
        "Hùng",
        "Lan",
        "Sơn",
        "Minh",
        "Bình",
        "Trang",
        "Dũng",
        "Hạnh",
        "Phúc",
        "Linh",
        "Tuấn",
        "Mai"
    ];

    private static readonly string[] ForeignFirstNames =
    [
        "John",
        "Yuki",
        "Park",
        "Michael",
        "Anna",
        "Sofia",
        "David",
        "Hana",
        "Liam",
        "Emma",
        "Lucas",
        "Mina"
    ];

    private static readonly string[] ForeignLastNames =
    [
        "Smith",
        "Tanaka",
        "Min-jun",
        "Johnson",
        "Muller",
        "Garcia",
        "Brown",
        "Kim",
        "Wilson",
        "Lee",
        "Martin",
        "Chen"
    ];

    private static readonly string[] Nationalities =
    [
        "Mỹ",
        "Nhật Bản",
        "Hàn Quốc",
        "Đức",
        "Pháp",
        "Úc",
        "Singapore",
        "Thái Lan",
        "Canada",
        "Anh"
    ];

    private static readonly string[] VisaTypes =
    [
        "Du lịch",
        "Lao động",
        "Học tập",
        "Đầu tư",
        "Công tác"
    ];

    private static readonly string[] StayPurposes =
    [
        "Du lịch",
        "Công tác",
        "Lao động ngắn hạn",
        "Học tập",
        "Thăm thân",
        "Khảo sát đầu tư"
    ];

    private static readonly string[] WarningTypes =
    [
        "Quá hạn lưu trú",
        "Chưa cập nhật nơi cư trú",
        "Thiếu thông tin khai báo",
        "Sai lệch thông tin giấy tờ",
        "Cần xác minh lưu trú"
    ];

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
        await SeedLichSuCapNhatAsync(context);
        await SeedViolationDataAsync(context);
    }

    public static IReadOnlyList<PhuongXa> BuildDaNangPhuongXaSeed()
    {
        return DaNangAdministrativeUnitNames
            .Select((name, index) => new PhuongXa
            {
                MaPhuongXa = index + 1,
                TenPhuongXa = name
            })
            .ToArray();
    }

    public static LargeSeedTargets GetLargeSeedTargets()
    {
        return new LargeSeedTargets(
            PhuongXaCount: DaNangAdministrativeUnitNames.Length,
            TaiKhoanCount: 9 + (TargetPoliceWardCoverage - 2) + (TargetOfficerCount - 1) + (TargetOwnerCount - 2) + (TargetForeignerCount - 3),
            CanBoCount: TargetPoliceWardCoverage + TargetOfficerCount,
            ChuCoSoCount: TargetOwnerCount,
            CoSoLuuTruCount: TargetFacilityCount,
            NguoiNuocNgoaiCount: TargetForeignerCount,
            HoSoKhaiBaoTamTruCount: TargetDeclarationCount,
            LichSuCuTruCount: TargetStayHistoryCount,
            LichSuCapNhatCount: TargetProfileUpdateCount,
            CanhBaoViPhamCount: TargetWarningCount,
            BaoCaoViPhamCount: TargetViolationReportCount);
    }

    private static async Task SeedPhuongXaAsync(ApplicationDbContext context)
    {
        var existing = await context.PhuongXas.ToDictionaryAsync(x => x.MaPhuongXa);

        foreach (var ward in BuildDaNangPhuongXaSeed())
        {
            if (existing.TryGetValue(ward.MaPhuongXa, out var current))
            {
                current.TenPhuongXa = ward.TenPhuongXa;
            }
            else
            {
                context.PhuongXas.Add(ward);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedVaiTroAsync(ApplicationDbContext context)
    {
        var roles = new[]
        {
            new VaiTro { MaVaiTro = 1, TenVaiTro = VaiTroConst.NguoiNuocNgoai, MoTaVaiTro = "Người nước ngoài", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 2, TenVaiTro = VaiTroConst.ChuLuuTru, MoTaVaiTro = "Chủ cơ sở lưu trú", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 3, TenVaiTro = VaiTroConst.CongAn, MoTaVaiTro = "Công an Phường/Xã", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 4, TenVaiTro = VaiTroConst.CanBoXNC, MoTaVaiTro = "Cán bộ quản lý xuất nhập cảnh", TrangThai = "Hoạt động" },
            new VaiTro { MaVaiTro = 5, TenVaiTro = VaiTroConst.Admin, MoTaVaiTro = "Quản trị viên hệ thống", TrangThai = "Hoạt động" }
        };

        var existing = await context.VaiTros.ToDictionaryAsync(x => x.MaVaiTro);
        foreach (var role in roles)
        {
            if (existing.TryGetValue(role.MaVaiTro, out var current))
            {
                current.TenVaiTro = role.TenVaiTro;
                current.MoTaVaiTro = role.MoTaVaiTro;
                current.TrangThai = role.TrangThai;
            }
            else
            {
                context.VaiTros.Add(role);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedQuyenHanAsync(ApplicationDbContext context)
    {
        var permissions = new[]
        {
            Permission(1, "Foreigner.Profile", "Tra cứu và cập nhật thông tin cá nhân"),
            Permission(1, "Foreigner.ResidenceDeclaration", "Khai báo tạm trú"),
            Permission(2, "Accommodation.StayDeclaration", "Khai báo và cập nhật trạng thái lưu trú"),
            Permission(2, "Accommodation.FacilityProfile", "Cập nhật thông tin cơ sở lưu trú"),
            Permission(3, "Police.ApproveDeclaration", "Phê duyệt hoặc từ chối khai báo tạm trú theo địa bàn"),
            Permission(3, "Police.ViolationReport", "Cảnh báo và báo cáo vi phạm theo địa bàn"),
            Permission(4, "Officer.SearchForeigner", "Tra cứu người nước ngoài toàn thành phố"),
            Permission(4, "Officer.ReportStatistics", "Thống kê, lập báo cáo và xử lý báo cáo vi phạm"),
            Permission(5, "Admin.AccountRole", "Quản lý tài khoản, vai trò, quyền và phường xã")
        };

        var existingNames = await context.QuyenHans
            .Select(x => x.TenQuyen)
            .ToListAsync();

        context.QuyenHans.AddRange(permissions.Where(p => !existingNames.Contains(p.TenQuyen)));
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
        var passwordHashes = new Dictionary<int, string>
        {
            [1] = BCrypt.Net.BCrypt.HashPassword("User@123"),
            [2] = BCrypt.Net.BCrypt.HashPassword("KhachSan@123"),
            [3] = BCrypt.Net.BCrypt.HashPassword("CongAn@123"),
            [4] = BCrypt.Net.BCrypt.HashPassword("CanBo@123"),
            [5] = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        };

        var accounts = BuildTaiKhoanSeed(passwordHashes).ToArray();
        var existing = (await context.TaiKhoans.ToListAsync()).ToDictionary(x => x.MaTaiKhoan.Trim());

        foreach (var account in accounts)
        {
            if (existing.TryGetValue(account.MaTaiKhoan, out var current))
            {
                current.MaVaiTro = account.MaVaiTro;
                current.Email = account.Email;
                current.SoDienThoai = account.SoDienThoai;
                current.TrangThai = account.TrangThai;
            }
            else
            {
                context.TaiKhoans.Add(account);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<TaiKhoan> BuildTaiKhoanSeed(IReadOnlyDictionary<int, string> passwordHashes)
    {
        yield return Account(1, "admin", 5, "admin@danang.gov.vn", "0236000001");
        yield return Account(2, "canbo01", 4, "canbo01@danang.gov.vn", "0236000002");
        yield return Account(3, "congan01", 3, "congan01@danang.gov.vn", "0236000003");
        yield return Account(4, "congan02", 3, "congan02@danang.gov.vn", "0236000004");
        yield return Account(5, "khachsan01", 2, "khachsan01@danang.vn", "0236100001");
        yield return Account(6, "nhatro01", 2, "nhatro01@danang.vn", "0236100002");
        yield return Account(7, "john.smith", 1, "john.smith@example.com", "0901234567");
        yield return Account(8, "yuki.tanaka", 1, "yuki.tanaka@example.com", "0901234568");
        yield return Account(9, "park.minjun", 1, "park.minjun@example.com", "0901234569");

        for (var wardId = 3; wardId <= TargetPoliceWardCoverage; wardId++)
        {
            var accountNumber = 10 + wardId - 3;
            yield return Account(accountNumber, $"congan{wardId:000}", 3, $"congan{wardId:000}@danang.gov.vn", $"02361{wardId:00000}");
        }

        for (var officerIndex = 2; officerIndex <= TargetOfficerCount; officerIndex++)
        {
            var accountNumber = 100 + officerIndex;
            yield return Account(accountNumber, $"canbo{officerIndex:00}", 4, $"canbo{officerIndex:00}@danang.gov.vn", $"02362{officerIndex:00000}");
        }

        for (var ownerIndex = 3; ownerIndex <= TargetOwnerCount; ownerIndex++)
        {
            var accountNumber = 200 + ownerIndex - 3;
            yield return Account(accountNumber, $"cosoluutru{ownerIndex:000}", 2, $"cosoluutru{ownerIndex:000}@danang.vn", $"091{ownerIndex:0000000}");
        }

        for (var foreignerIndex = 4; foreignerIndex <= TargetForeignerCount; foreignerIndex++)
        {
            var accountNumber = 1000 + foreignerIndex - 4;
            yield return Account(accountNumber, $"foreign{foreignerIndex:000}", 1, $"foreign{foreignerIndex:000}@example.com", $"084{foreignerIndex:0000000}");
        }

        TaiKhoan Account(int number, string username, int roleId, string email, string phone)
        {
            return new TaiKhoan
            {
                MaTaiKhoan = FormatId("TK", number),
                TenDangNhap = username,
                MatKhauHash = passwordHashes[roleId],
                MaVaiTro = roleId,
                Email = email,
                SoDienThoai = phone,
                TrangThai = TrangThaiTaiKhoan.HoatDong,
                NgayTao = DateTime.Now.AddDays(-(number % 365))
            };
        }
    }

    private static async Task SeedCanBoAsync(ApplicationDbContext context)
    {
        var staff = BuildCanBoSeed().ToArray();
        var existing = (await context.CanBos.ToListAsync()).ToDictionary(x => x.MaCanBo.Trim());

        foreach (var item in staff)
        {
            if (existing.TryGetValue(item.MaCanBo, out var current))
            {
                current.MaPhuongXa = item.MaPhuongXa;
                current.MaTaiKhoan = item.MaTaiKhoan;
                current.HoTen = item.HoTen;
                current.DonViCongTac = item.DonViCongTac;
                current.ChucVu = item.ChucVu;
                current.CapQuanLy = item.CapQuanLy;
                current.TrangThai = item.TrangThai;
            }
            else
            {
                context.CanBos.Add(item);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CanBo> BuildCanBoSeed()
    {
        yield return Staff("CB000001", 1, "TK000003", "Nguyễn Văn Hùng", "048100000001", "Công an Phường Hải Châu", "Cán bộ quản lý cư trú", "Phường");
        yield return Staff("CB000002", 2, "TK000004", "Trần Thị Lan", "048100000002", "Công an Phường Hòa Cường", "Cán bộ quản lý cư trú", "Phường");
        yield return Staff("CB000003", 1, "TK000002", "Lê Hải Sơn", "048100000003", "Phòng Quản lý Xuất nhập cảnh TP Đà Nẵng", "Cán bộ chuyên trách", "Thành phố");

        for (var wardId = 3; wardId <= TargetPoliceWardCoverage; wardId++)
        {
            var accountNumber = 10 + wardId - 3;
            yield return Staff(
                WardPoliceCanBoId(wardId),
                wardId,
                FormatId("TK", accountNumber),
                VietnameseName(wardId),
                $"0481{wardId:00000000}",
                $"Công an {DaNangAdministrativeUnitNames[wardId - 1]}",
                "Cán bộ quản lý cư trú",
                DaNangAdministrativeUnitNames[wardId - 1].StartsWith("Phường ", StringComparison.Ordinal) ? "Phường" : "Xã");
        }

        for (var officerIndex = 2; officerIndex <= TargetOfficerCount; officerIndex++)
        {
            yield return Staff(
                FormatId("CB", officerIndex + 2),
                officerIndex,
                FormatId("TK", 100 + officerIndex),
                VietnameseName(100 + officerIndex),
                $"0482{officerIndex:00000000}",
                "Phòng Quản lý Xuất nhập cảnh TP Đà Nẵng",
                "Cán bộ chuyên trách",
                "Thành phố");
        }

        static CanBo Staff(string id, int wardId, string accountId, string name, string citizenId, string unit, string position, string managementLevel)
        {
            var seed = int.Parse(id[^3..]);
            return new CanBo
            {
                MaCanBo = id,
                MaPhuongXa = wardId,
                MaTaiKhoan = accountId,
                HoTen = name,
                SoCCCD = citizenId,
                NgayCapCCCD = new DateTime(2018, 1, 1).AddDays(seed),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = $"{10 + seed % 200} {StreetNames[seed % StreetNames.Length]}, Đà Nẵng",
                NgaySinh = new DateTime(1978 + seed % 18, 1 + seed % 12, 1 + seed % 27),
                GioiTinh = seed % 2 == 0 ? "Nữ" : "Nam",
                DonViCongTac = unit,
                ChucVu = position,
                CapQuanLy = managementLevel,
                TrangThai = "Hoạt động"
            };
        }
    }

    private static async Task SeedChuCoSoAsync(ApplicationDbContext context)
    {
        var owners = BuildChuCoSoSeed().ToArray();
        var existing = (await context.ChuCoSoLuuTrus.ToListAsync()).ToDictionary(x => x.MaChuCoSo.Trim());

        foreach (var owner in owners)
        {
            if (existing.TryGetValue(owner.MaChuCoSo, out var current))
            {
                current.MaTaiKhoan = owner.MaTaiKhoan;
                current.HoTen = owner.HoTen;
                current.DiaChiThuongTru = owner.DiaChiThuongTru;
            }
            else
            {
                context.ChuCoSoLuuTrus.Add(owner);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<ChuCoSoLuuTru> BuildChuCoSoSeed()
    {
        for (var index = 1; index <= TargetOwnerCount; index++)
        {
            var accountId = index switch
            {
                1 => "TK000005",
                2 => "TK000006",
                _ => FormatId("TK", 200 + index - 3)
            };

            yield return new ChuCoSoLuuTru
            {
                MaChuCoSo = FormatId("CC", index),
                MaTaiKhoan = accountId,
                HoTen = index switch
                {
                    1 => "Phạm Quang Minh",
                    2 => "Nguyễn Thị Bình",
                    _ => VietnameseName(index + 40)
                },
                NgaySinh = new DateTime(1970 + index % 25, 1 + index % 12, 1 + index % 27),
                GioiTinh = index % 2 == 0 ? "Nữ" : "Nam",
                SoCCCD = $"0483{index:00000000}",
                NgayCapCCCD = new DateTime(2016, 1, 1).AddDays(index),
                NoiCapCCCD = "Cục Cảnh sát QLHC về TTXH",
                DiaChiThuongTru = $"{20 + index % 220} {StreetNames[index % StreetNames.Length]}, Đà Nẵng"
            };
        }
    }

    private static async Task SeedCoSoAsync(ApplicationDbContext context)
    {
        var facilities = BuildCoSoSeed().ToArray();
        var existing = (await context.CoSoLuuTrus.ToListAsync()).ToDictionary(x => x.MaCoSoLuuTru.Trim());

        foreach (var facility in facilities)
        {
            if (existing.TryGetValue(facility.MaCoSoLuuTru, out var current))
            {
                current.MaPhuongXa = facility.MaPhuongXa;
                current.MaChuCoSo = facility.MaChuCoSo;
                current.TenCoSo = facility.TenCoSo;
                current.DiaChi = facility.DiaChi;
                current.SoDienThoai = facility.SoDienThoai;
                current.Email = facility.Email;
                current.TrangThai = facility.TrangThai;
            }
            else
            {
                context.CoSoLuuTrus.Add(facility);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CoSoLuuTru> BuildCoSoSeed()
    {
        var facilityTypes = new[] { "Khách sạn", "Căn hộ dịch vụ", "Nhà nghỉ", "Homestay", "Khu lưu trú" };

        for (var index = 1; index <= TargetFacilityCount; index++)
        {
            var wardId = 1 + (index - 1) % DaNangAdministrativeUnitNames.Length;
            var ownerId = 1 + (index - 1) % TargetOwnerCount;
            var wardName = DaNangAdministrativeUnitNames[wardId - 1];
            var type = facilityTypes[index % facilityTypes.Length];

            yield return new CoSoLuuTru
            {
                MaCoSoLuuTru = FormatId("CS", index),
                MaPhuongXa = wardId,
                MaChuCoSo = FormatId("CC", ownerId),
                TenCoSo = $"{type} Đà Nẵng {index:000}",
                DiaChi = $"{30 + index % 260} {StreetNames[index % StreetNames.Length]}, {wardName}, Đà Nẵng",
                SoDienThoai = $"0236{index:0000000}",
                Email = $"cosoluutru{index:000}@danang.vn",
                TrangThai = index % 17 == 0 ? TrangThaiCoSo.DaNgungHoatDong : TrangThaiCoSo.DangHoatDong
            };
        }
    }

    private static async Task SeedNguoiNuocNgoaiAsync(ApplicationDbContext context)
    {
        var foreigners = BuildNguoiNuocNgoaiSeed().ToArray();
        var existing = (await context.NguoiNuocNgoais.ToListAsync()).ToDictionary(x => x.MaNguoiNuocNgoai.Trim());

        foreach (var foreigner in foreigners)
        {
            if (existing.TryGetValue(foreigner.MaNguoiNuocNgoai, out var current))
            {
                current.MaTaiKhoan = foreigner.MaTaiKhoan;
                current.HoTen = foreigner.HoTen;
                current.QuocTich = foreigner.QuocTich;
                current.LoaiVisa = foreigner.LoaiVisa;
                current.NgayHetHanVisa = foreigner.NgayHetHanVisa;
            }
            else
            {
                context.NguoiNuocNgoais.Add(foreigner);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<NguoiNuocNgoai> BuildNguoiNuocNgoaiSeed()
    {
        for (var index = 1; index <= TargetForeignerCount; index++)
        {
            var nationality = Nationalities[(index - 1) % Nationalities.Length];
            var visaType = VisaTypes[(index - 1) % VisaTypes.Length];

            yield return new NguoiNuocNgoai
            {
                MaNguoiNuocNgoai = FormatId("NN", index),
                MaTaiKhoan = ForeignerAccountId(index),
                HoTen = index switch
                {
                    1 => "John Smith",
                    2 => "Yuki Tanaka",
                    3 => "Park Min-jun",
                    _ => $"{ForeignFirstNames[index % ForeignFirstNames.Length]} {ForeignLastNames[index % ForeignLastNames.Length]} {index:000}"
                },
                NgaySinh = new DateTime(1970 + index % 35, 1 + index % 12, 1 + index % 27),
                GioiTinh = index % 3 == 0 ? "Nữ" : "Nam",
                QuocTich = nationality,
                SoHoChieu = PassportNumber(index, nationality),
                NgayCapHoChieu = new DateTime(2021, 1, 1).AddDays(index % 700),
                NgayHetHanHoChieu = new DateTime(2031, 1, 1).AddDays(index % 700),
                LoaiVisa = visaType,
                NgayHetHanVisa = DateTime.Today.AddDays(30 + index % 720)
            };
        }
    }

    private static async Task SeedHoSoTamTruAsync(ApplicationDbContext context)
    {
        var declarations = BuildHoSoTamTruSeed().ToArray();
        var existing = (await context.HoSoKhaiBaoTamTrus.ToListAsync()).ToDictionary(x => x.MaHSKhaiBao.Trim());

        foreach (var declaration in declarations)
        {
            if (existing.TryGetValue(declaration.MaHSKhaiBao, out var current))
            {
                current.MaTaiKhoan = declaration.MaTaiKhoan;
                current.MaCoSoLuuTru = declaration.MaCoSoLuuTru;
                current.NgayBatDau = declaration.NgayBatDau;
                current.NgayKetThuc = declaration.NgayKetThuc;
                current.MucDichLuuTru = declaration.MucDichLuuTru;
                current.DiaChiLuuTru = declaration.DiaChiLuuTru;
                current.TrangThai = declaration.TrangThai;
                current.LyDoTuChoi = declaration.LyDoTuChoi;
                current.GhiChu = declaration.GhiChu;
            }
            else
            {
                context.HoSoKhaiBaoTamTrus.Add(declaration);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<HoSoKhaiBaoTamTru> BuildHoSoTamTruSeed()
    {
        for (var index = 1; index <= TargetDeclarationCount; index++)
        {
            var foreignerIndex = 1 + (index - 1) % TargetForeignerCount;
            var facilityIndex = 1 + (index - 1) % TargetFacilityCount;
            var wardId = 1 + (facilityIndex - 1) % DaNangAdministrativeUnitNames.Length;
            var start = DateTime.Today.AddDays(-(index % 120));
            var status = index % 10 == 0
                ? TrangThaiKhaiBao.TuChoi
                : index % 3 == 0
                    ? TrangThaiKhaiBao.DaDuyet
                    : TrangThaiKhaiBao.ChoDuyet;

            yield return new HoSoKhaiBaoTamTru
            {
                MaHSKhaiBao = FormatId("HS", index),
                MaTaiKhoan = ForeignerAccountId(foreignerIndex),
                MaCoSoLuuTru = FormatId("CS", facilityIndex),
                NgayKhaiBao = start.AddDays(-1),
                NgayBatDau = start,
                NgayKetThuc = start.AddDays(7 + index % 180),
                MucDichLuuTru = StayPurposes[index % StayPurposes.Length],
                DiaChiLuuTru = $"{30 + facilityIndex % 260} {StreetNames[facilityIndex % StreetNames.Length]}, {DaNangAdministrativeUnitNames[wardId - 1]}, Đà Nẵng",
                TrangThai = status,
                LyDoTuChoi = status == TrangThaiKhaiBao.TuChoi ? "Thông tin lưu trú chưa khớp với hồ sơ cơ sở lưu trú." : null,
                GhiChu = $"Hồ sơ seed phục vụ kiểm thử #{index:000}"
            };
        }
    }

    private static async Task SeedLichSuCuTruAsync(ApplicationDbContext context)
    {
        var stays = BuildLichSuCuTruSeed().ToArray();
        var existing = (await context.LichSuCuTrus.ToListAsync()).ToDictionary(x => x.MaLSLuuTru.Trim());

        foreach (var stay in stays)
        {
            if (existing.TryGetValue(stay.MaLSLuuTru, out var current))
            {
                current.MaNguoiNuocNgoai = stay.MaNguoiNuocNgoai;
                current.MaCoSoLuuTru = stay.MaCoSoLuuTru;
                current.NgayBatDau = stay.NgayBatDau;
                current.NgayKetThuc = stay.NgayKetThuc;
                current.Phong = stay.Phong;
                current.TrangThai = stay.TrangThai;
                current.GhiChu = stay.GhiChu;
            }
            else
            {
                context.LichSuCuTrus.Add(stay);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<LichSuCuTru> BuildLichSuCuTruSeed()
    {
        for (var index = 1; index <= TargetStayHistoryCount; index++)
        {
            var start = DateTime.Now.AddDays(-(index % 180));
            var status = index % 9 == 0
                ? TrangThaiLuuTru.QuaHan
                : index % 4 == 0
                    ? TrangThaiLuuTru.DaRoi
                    : TrangThaiLuuTru.DangO;

            yield return new LichSuCuTru
            {
                MaLSLuuTru = FormatId("LS", index),
                MaNguoiNuocNgoai = FormatId("NN", 1 + (index - 1) % TargetForeignerCount),
                MaCoSoLuuTru = FormatId("CS", 1 + (index - 1) % TargetFacilityCount),
                NgayBatDau = start,
                NgayKetThuc = status == TrangThaiLuuTru.DangO ? null : start.AddDays(2 + index % 60),
                Phong = $"{(index % 20) + 1:00}{(index % 30) + 1:00}",
                TrangThai = status,
                GhiChu = $"Dữ liệu lịch sử lưu trú mẫu #{index:000}"
            };
        }
    }

    private static async Task SeedLichSuCapNhatAsync(ApplicationDbContext context)
    {
        var updates = BuildLichSuCapNhatSeed().ToArray();
        var existing = (await context.LichSuCapNhatThongTinCaNhans.ToListAsync()).ToDictionary(x => x.MaLSCapNhat.Trim());

        foreach (var update in updates)
        {
            if (!existing.ContainsKey(update.MaLSCapNhat))
            {
                context.LichSuCapNhatThongTinCaNhans.Add(update);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<LichSuCapNhatThongTinCaNhan> BuildLichSuCapNhatSeed()
    {
        var fields = new[] { "Email", "Số điện thoại", "Loại visa", "Ngày hết hạn visa", "Địa chỉ liên hệ" };

        for (var index = 1; index <= TargetProfileUpdateCount; index++)
        {
            yield return new LichSuCapNhatThongTinCaNhan
            {
                MaLSCapNhat = FormatId("CN", index),
                MaTaiKhoan = ForeignerAccountId(1 + (index - 1) % TargetForeignerCount),
                TruongCapNhat = fields[index % fields.Length],
                GiaTriCu = $"Giá trị cũ {index:000}",
                GiaTriMoi = $"Giá trị mới {index:000}",
                NgayCapNhat = DateTime.Now.AddDays(-(index % 90)),
                LyDoCapNhat = "Seed dữ liệu lịch sử cập nhật cá nhân",
                TrangThai = "Đã cập nhật"
            };
        }
    }

    private static async Task SeedViolationDataAsync(ApplicationDbContext context)
    {
        var warningCount = await context.CanhBaoViPhams.CountAsync();
        if (warningCount < TargetWarningCount)
        {
            context.CanhBaoViPhams.AddRange(BuildCanhBaoSeed(warningCount + 1, TargetWarningCount));
            await context.SaveChangesAsync();
        }

        // Sửa lỗi hiển thị các bản ghi cảnh báo cũ bị lỗi font do cột text trước đó
        var existingWarnings = await context.CanhBaoViPhams.ToListAsync();
        var warningsUpdated = false;
        foreach (var warn in existingWarnings)
        {
            var seedIdx = warn.NoiDungCanhBao.IndexOf("seed #");
            if (seedIdx >= 0 && seedIdx + 6 + 3 <= warn.NoiDungCanhBao.Length)
            {
                var indexStr = warn.NoiDungCanhBao.Substring(seedIdx + 6, 3);
                if (int.TryParse(indexStr, out int index))
                {
                    var correctContent = $"Cảnh báo seed #{index:000}: cần rà soát thông tin lưu trú và giấy tờ liên quan.";
                    var correctGhiChu = "Dữ liệu cảnh báo mẫu phục vụ kiểm thử nghiệp vụ.";
                    if (warn.NoiDungCanhBao != correctContent || warn.GhiChu != correctGhiChu)
                    {
                        warn.NoiDungCanhBao = correctContent;
                        warn.GhiChu = correctGhiChu;
                        warningsUpdated = true;
                    }
                }
            }
            else if (warn.NoiDungCanhBao.Contains("?"))
            {
                warn.NoiDungCanhBao = warn.NoiDungCanhBao
                    .Replace("C?nh báo", "Cảnh báo")
                    .Replace("c?n rà soát", "cần rà soát")
                    .Replace("gi?y t?", "giấy tờ")
                    .Replace("luu trú", "lưu trú");
                warningsUpdated = true;
            }
        }
        if (warningsUpdated)
        {
            await context.SaveChangesAsync();
        }

        var reports = BuildBaoCaoSeed().ToArray();
        var existingReports = (await context.BaoCaoViPhams.ToListAsync()).ToDictionary(x => x.MaBaoCao.Trim());

        foreach (var report in reports)
        {
            if (existingReports.TryGetValue(report.MaBaoCao, out var current))
            {
                current.MaNguoiNuocNgoai = report.MaNguoiNuocNgoai;
                current.MaCanBo = report.MaCanBo;
                current.NoiDungBaoCao = report.NoiDungBaoCao;
                current.TrangThaiXuLy = report.TrangThaiXuLy;
            }
            else
            {
                context.BaoCaoViPhams.Add(report);
            }
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CanhBaoViPham> BuildCanhBaoSeed(int start, int end)
    {
        for (var index = start; index <= end; index++)
        {
            var severity = index % 7 == 0
                ? MucDoViPhamConst.NghiemTrong
                : index % 3 == 0
                    ? MucDoViPhamConst.TrungBinh
                    : MucDoViPhamConst.Nhe;

            yield return new CanhBaoViPham
            {
                MaNguoiNuocNgoai = FormatId("NN", 1 + (index - 1) % TargetForeignerCount),
                MaCanBo = index % 2 == 0 ? OfficerCanBoId(index) : WardPoliceCanBoId(1 + (index - 1) % TargetPoliceWardCoverage),
                LoaiViPham = WarningTypes[index % WarningTypes.Length],
                NoiDungCanhBao = $"Cảnh báo seed #{index:000}: cần rà soát thông tin lưu trú và giấy tờ liên quan.",
                MucDoViPham = severity,
                NgayCanhBao = DateTime.Now.AddDays(-(index % 60)),
                TrangThai = index % 5 == 0 ? TrangThaiCanhBao.DangXuLy : TrangThaiCanhBao.DaGui,
                GhiChu = "Dữ liệu cảnh báo mẫu phục vụ kiểm thử nghiệp vụ."
            };
        }
    }

    private static IEnumerable<BaoCaoViPham> BuildBaoCaoSeed()
    {
        for (var index = 1; index <= TargetViolationReportCount; index++)
        {
            yield return new BaoCaoViPham
            {
                MaBaoCao = FormatId("BC", index),
                MaNguoiNuocNgoai = FormatId("NN", 1 + (index - 1) % TargetForeignerCount),
                MaCanBo = WardPoliceCanBoId(1 + (index - 1) % TargetPoliceWardCoverage),
                NoiDungBaoCao = $"Báo cáo seed #{index:000}: đề nghị cán bộ xuất nhập cảnh rà soát dấu hiệu vi phạm lưu trú.",
                NgayBaoCao = DateTime.Now.AddDays(-(index % 45)),
                TrangThaiXuLy = index % 6 == 0
                    ? TrangThaiXuLyConst.DaXuLy
                    : index % 3 == 0
                        ? TrangThaiXuLyConst.DangXuLy
                        : TrangThaiXuLyConst.ChuaXuLy
            };
        }
    }

    private static string FormatId(string prefix, int number)
    {
        return $"{prefix}{number:D6}";
    }

    private static string ForeignerAccountId(int foreignerIndex)
    {
        return foreignerIndex switch
        {
            1 => "TK000007",
            2 => "TK000008",
            3 => "TK000009",
            _ => FormatId("TK", 1000 + foreignerIndex - 4)
        };
    }

    private static string WardPoliceCanBoId(int wardId)
    {
        return wardId switch
        {
            1 => "CB000001",
            2 => "CB000002",
            _ => FormatId("CB", 100 + wardId)
        };
    }

    private static string OfficerCanBoId(int seed)
    {
        var officerNumber = 3 + seed % TargetOfficerCount;
        return FormatId("CB", officerNumber);
    }

    private static string VietnameseName(int seed)
    {
        return $"{VietnameseFamilyNames[seed % VietnameseFamilyNames.Length]} {VietnameseMiddleNames[(seed / 2) % VietnameseMiddleNames.Length]} {VietnameseGivenNames[(seed / 3) % VietnameseGivenNames.Length]}";
    }

    private static string PassportNumber(int index, string nationality)
    {
        var prefix = nationality switch
        {
            "Mỹ" => "US",
            "Nhật Bản" => "JP",
            "Hàn Quốc" => "KR",
            "Đức" => "DE",
            "Pháp" => "FR",
            "Úc" => "AU",
            "Singapore" => "SG",
            "Thái Lan" => "TH",
            "Canada" => "CA",
            "Anh" => "GB",
            _ => "VN"
        };

        return $"{prefix}{index:00000000}";
    }
}

public sealed record LargeSeedTargets(
    int PhuongXaCount,
    int TaiKhoanCount,
    int CanBoCount,
    int ChuCoSoCount,
    int CoSoLuuTruCount,
    int NguoiNuocNgoaiCount,
    int HoSoKhaiBaoTamTruCount,
    int LichSuCuTruCount,
    int LichSuCapNhatCount,
    int CanhBaoViPhamCount,
    int BaoCaoViPhamCount);
