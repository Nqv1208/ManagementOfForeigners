using ManagementOfForeigners.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

var tests = new List<(string Name, Action Test)>
{
    ("ERD maps PhuongXa and ward-scoped foreign keys", VerifyWardSchema),
    ("ERD strict entity shapes are enforced", VerifyStrictErdEntityShapes),
    ("Residence declarations require an accommodation facility", VerifyDeclarationFacilityRequired),
    ("Seed data contains the 94 new Da Nang administrative units", VerifyDaNangAdministrativeUnits),
    ("Large seed plan covers operational tables", VerifyLargeSeedTargets),
    ("Facility selection uses bounded autocomplete instead of large selects", VerifyFacilityAutocomplete),
    ("Auth subsystem uses the dedicated government auth design", VerifyAuthSubsystem),
    ("UI theme exposes the approved green government palette", VerifyGreenThemeTokens),
    ("UI theme does not keep legacy blue theme values", VerifyNoLegacyBlueThemeValues),
    ("Razor views do not use legacy dark/info Bootstrap branding", VerifyNoLegacyBootstrapBrandClasses)
};

var failures = new List<string>();

foreach (var (name, test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: {ex.Message}");
        Console.WriteLine($"FAIL {name}: {ex.Message}");
    }
}

if (failures.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Schema verification failed:");
    foreach (var failure in failures)
    {
        Console.WriteLine($"- {failure}");
    }

    Environment.Exit(1);
}

static ApplicationDbContext CreateContext()
{
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("Server=localhost,1433;Database=SchemaVerification;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=True;")
        .Options;

    return new ApplicationDbContext(options);
}

static void VerifyWardSchema()
{
    using var context = CreateContext();
    var model = context.Model;

    var phuongXaType = Type.GetType("ManagementOfForeigners.Models.Entities.PhuongXa, ManagementOfForeigners");
    Assert(phuongXaType is not null, "PhuongXa entity type must exist.");
    Assert(typeof(ApplicationDbContext).GetProperty("PhuongXas") is not null, "ApplicationDbContext must expose DbSet<PhuongXa> PhuongXas.");

    var phuongXaEntity = model.FindEntityType(phuongXaType!);
    if (phuongXaEntity is null)
    {
        throw new InvalidOperationException("PhuongXa must be mapped by EF Core.");
    }

    Assert(phuongXaEntity.GetTableName() == "PhuongXa", "PhuongXa table name must be singular.");
    var primaryKey = phuongXaEntity.FindPrimaryKey();
    Assert(primaryKey is not null, "PhuongXa must have a primary key.");
    Assert(primaryKey!.Properties.Single().Name == "MaPhuongXa", "PhuongXa primary key must be MaPhuongXa.");
    Assert(phuongXaEntity.FindProperty("TenPhuongXa")?.GetMaxLength() == 100, "TenPhuongXa must be nvarchar(100).");
    Assert(phuongXaEntity.FindProperty("QuanHuyen") is null, "PhuongXa must not map QuanHuyen.");

    AssertHasRequiredWardForeignKey(model, "ManagementOfForeigners.Models.Entities.CanBo", "CanBo");
    AssertHasRequiredWardForeignKey(model, "ManagementOfForeigners.Models.Entities.CoSoLuuTru", "CoSoLuuTru");
}

static void VerifyDeclarationFacilityRequired()
{
    using var context = CreateContext();
    var declarationType = Type.GetType("ManagementOfForeigners.Models.Entities.HoSoKhaiBaoTamTru, ManagementOfForeigners");
    Assert(declarationType is not null, "HoSoKhaiBaoTamTru entity type must exist.");

    var entity = context.Model.FindEntityType(declarationType!);
    Assert(entity is not null, "HoSoKhaiBaoTamTru must be mapped by EF Core.");

    var facilityKey = entity!.FindProperty("MaCoSoLuuTru");
    Assert(facilityKey is not null, "HoSoKhaiBaoTamTru must keep MaCoSoLuuTru.");
    Assert(facilityKey!.IsNullable == false, "MaCoSoLuuTru must be required so ward approval can be scoped through CoSoLuuTru.MaPhuongXa.");
}

static void VerifyStrictErdEntityShapes()
{
    using var context = CreateContext();

    AssertMappedProperties(
        context,
        "ManagementOfForeigners.Models.Entities.CoSoLuuTru",
        "CoSoLuuTru",
        [
            "MaCoSoLuuTru",
            "MaPhuongXa",
            "MaChuCoSo",
            "TenCoSo",
            "DiaChi",
            "SoDienThoai",
            "Email",
            "TrangThai"
        ]);

    AssertMappedProperties(
        context,
        "ManagementOfForeigners.Models.Entities.NguoiNuocNgoai",
        "NguoiNuocNgoai",
        [
            "MaNguoiNuocNgoai",
            "MaTaiKhoan",
            "HoTen",
            "NgaySinh",
            "GioiTinh",
            "QuocTich",
            "SoHoChieu",
            "NgayCapHoChieu",
            "NgayHetHanHoChieu",
            "LoaiVisa",
            "NgayHetHanVisa"
        ]);

    var foreignerType = Type.GetType("ManagementOfForeigners.Models.Entities.NguoiNuocNgoai, ManagementOfForeigners");
    Assert(foreignerType is not null, "NguoiNuocNgoai entity type must exist.");
    Assert(foreignerType!.GetProperty("NoiCuTruHienTai") is null, "NoiCuTruHienTai must be computed in view models, not kept on the ERD entity.");

    var foreignerEntity = context.Model.FindEntityType(foreignerType);
    Assert(foreignerEntity is not null, "NguoiNuocNgoai must be mapped by EF Core.");
    var accountKey = foreignerEntity!.FindProperty("MaTaiKhoan");
    Assert(accountKey is not null, "NguoiNuocNgoai must keep MaTaiKhoan.");
    Assert(accountKey!.IsNullable, "NguoiNuocNgoai.MaTaiKhoan must remain nullable for accommodation-declared foreigners without accounts.");
}

static void VerifyDaNangAdministrativeUnits()
{
    var wards = SeedData.BuildDaNangPhuongXaSeed().ToList();

    Assert(wards.Count == 94, "Da Nang seed must contain 94 commune-level administrative units.");
    Assert(wards.Select(x => x.MaPhuongXa).Order().SequenceEqual(Enumerable.Range(1, 94)), "Da Nang administrative unit ids must be stable from 1 to 94.");
    Assert(wards.Select(x => x.TenPhuongXa).Distinct().Count() == 94, "Da Nang administrative unit names must be unique.");
    Assert(wards.Count(x => x.TenPhuongXa.StartsWith("Phường ", StringComparison.Ordinal)) == 23, "Da Nang seed must contain 23 wards.");
    Assert(wards.Count(x => x.TenPhuongXa.StartsWith("Xã ", StringComparison.Ordinal)) == 70, "Da Nang seed must contain 70 communes.");
    Assert(wards.Count(x => x.TenPhuongXa.StartsWith("Đặc khu ", StringComparison.Ordinal)) == 1, "Da Nang seed must contain 1 special zone.");
    Assert(wards[0].TenPhuongXa == "Phường Hải Châu", "The first Da Nang administrative unit must be Phường Hải Châu.");
    Assert(wards[^1].TenPhuongXa == "Xã Tân Hiệp", "The final Da Nang administrative unit must be Xã Tân Hiệp.");
}

static void VerifyLargeSeedTargets()
{
    var targets = SeedData.GetLargeSeedTargets();

    Assert(targets.PhuongXaCount == 94, "Large seed target must include all 94 Da Nang administrative units.");
    Assert(targets.TaiKhoanCount >= 600, "Large seed target must include at least 600 accounts.");
    Assert(targets.CanBoCount >= 90, "Large seed target must include ward police/officer coverage.");
    Assert(targets.ChuCoSoCount >= 100, "Large seed target must include at least 100 accommodation owners.");
    Assert(targets.CoSoLuuTruCount >= 150, "Large seed target must include at least 150 accommodation facilities.");
    Assert(targets.NguoiNuocNgoaiCount >= 300, "Large seed target must include at least 300 foreigners.");
    Assert(targets.HoSoKhaiBaoTamTruCount >= 500, "Large seed target must include at least 500 residence declarations.");
    Assert(targets.LichSuCuTruCount >= 300, "Large seed target must include at least 300 stay history rows.");
    Assert(targets.CanhBaoViPhamCount >= 100, "Large seed target must include at least 100 violation warnings.");
    Assert(targets.BaoCaoViPhamCount >= 80, "Large seed target must include at least 80 violation reports.");
}

static void VerifyFacilityAutocomplete()
{
    var script = ReadRepoFile("wwwroot/js/khai-bao-tam-tru.js");
    var css = ReadRepoFile("wwwroot/css/khai-bao-tam-tru.css");
    var declarationView = ReadRepoFile("Views/NguoiNuocNgoai/KhaiBaoTamTru.cshtml");
    var residenceView = ReadRepoFile("Views/NguoiNuocNgoai/LichSuCuTru.cshtml");
    var controller = ReadRepoFile("Controllers/NguoiNuocNgoaiController.cs");

    Assert(controller.Contains("/KhaiBaoTamTru/SearchCoSoLuuTru", StringComparison.Ordinal),
        "Controller must expose GET /KhaiBaoTamTru/SearchCoSoLuuTru.");
    Assert(script.Contains("/KhaiBaoTamTru/SearchCoSoLuuTru", StringComparison.Ordinal),
        "Facility autocomplete must call /KhaiBaoTamTru/SearchCoSoLuuTru.");
    Assert(!script.Contains("/Foreigner/SearchCoSoLuuTru", StringComparison.Ordinal),
        "Temporary residence autocomplete must not call the old action-style URL /Foreigner/SearchCoSoLuuTru.");
    Assert(script.Contains("query.length < 2", StringComparison.Ordinal),
        "Facility autocomplete must wait until the user enters at least 2 characters.");
    Assert(script.Contains("300", StringComparison.Ordinal),
        "Facility autocomplete must debounce search requests by 300ms.");
    Assert(css.Contains("max-height: 280px;", StringComparison.Ordinal),
        "Facility autocomplete dropdown must be bounded to about 280px and scroll internally.");
    Assert(css.Contains("overflow-y: auto;", StringComparison.Ordinal),
        "Facility autocomplete dropdown must scroll internally.");
    Assert(declarationView.Contains("type=\"hidden\" asp-for=\"CoSoLuuTruId\"", StringComparison.Ordinal),
        "Temporary residence declaration must keep a hidden CoSoLuuTruId input.");
    Assert(!declarationView.Contains("<select asp-for=\"CoSoLuuTruId\"", StringComparison.Ordinal) &&
           !declarationView.Contains("asp-items=\"ViewBag.CoSoLuuTrus\"", StringComparison.Ordinal),
        "Temporary residence declaration must not render accommodation facilities into a native select.");
    Assert(residenceView.Contains("type=\"hidden\" name=\"MaCoSoLuuTru\"", StringComparison.Ordinal),
        "Residence update form must keep a hidden MaCoSoLuuTru input.");
    Assert(!residenceView.Contains("<select name=\"MaCoSoLuuTru\"", StringComparison.Ordinal) &&
           !residenceView.Contains("ViewBag.CoSoLuuTrus", StringComparison.Ordinal),
        "Residence update form must not render accommodation facilities into a native select.");
}

static void VerifyAuthSubsystem()
{
    var layout = ReadRepoFile("Views/Shared/_AuthLayout.cshtml");
    var login = ReadRepoFile("Views/TaiKhoan/DangNhap.cshtml");
    var register = ReadRepoFile("Views/TaiKhoan/DangKy.cshtml");
    var registerSuccess = ReadRepoFile("Views/TaiKhoan/RegisterSuccess.cshtml");
    var pendingApproval = ReadRepoFile("Views/TaiKhoan/PendingApproval.cshtml");
    var controller = ReadRepoFile("Controllers/TaiKhoanController.cs");
    var authTheme = ReadRepoFile("wwwroot/css/auth-theme.css");
    var authPages = ReadRepoFile("wwwroot/css/auth-pages.css");
    var authJs = ReadRepoFile("wwwroot/js/auth-interactions.js");

    Assert(layout.Contains("auth-theme.css", StringComparison.Ordinal), "Auth layout must load auth-theme.css.");
    Assert(layout.Contains("auth-pages.css", StringComparison.Ordinal), "Auth layout must load auth-pages.css.");
    Assert(layout.Contains("auth-interactions.js", StringComparison.Ordinal), "Auth layout must load auth-interactions.js.");
    Assert(!layout.Contains("landing.css", StringComparison.Ordinal) && !layout.Contains("landing.js", StringComparison.Ordinal),
        "Auth layout must not depend on landing assets.");

    foreach (var (name, content) in new[]
    {
        ("Login", login),
        ("Register", register),
        ("RegisterSuccess", registerSuccess),
        ("PendingApproval", pendingApproval)
    })
    {
        Assert(content.Contains("Layout = \"_AuthLayout\"", StringComparison.Ordinal),
            $"{name} page must use the dedicated auth layout.");
    }

    Assert(login.Contains("Tên đăng nhập hoặc email", StringComparison.Ordinal), "Login page must support username or email microcopy.");
    Assert(!login.Contains("Tài khoản kiểm thử", StringComparison.Ordinal) || login.Contains("EnvironmentName", StringComparison.Ordinal),
        "Seeded test accounts must be hidden behind a Development environment guard.");
    Assert(!login.Contains("ForgotPassword", StringComparison.Ordinal), "Forgot password route must stay hidden in this phase.");

    Assert(register.Contains("data-auth-account-type=\"Foreigner\"", StringComparison.Ordinal) &&
           register.Contains("data-auth-account-type=\"LodgingOwner\"", StringComparison.Ordinal),
        "Register page must use a segmented account-type switch.");
    Assert(!register.Contains("register-unified.css", StringComparison.Ordinal), "Register page must not keep the old one-off register stylesheet.");
    Assert(!register.Contains("<script type=\"text/javascript\">", StringComparison.Ordinal), "Register page must not keep inline interaction JavaScript.");
    Assert(!register.Contains("type=\"file\"", StringComparison.OrdinalIgnoreCase) &&
           !register.Contains("QR", StringComparison.OrdinalIgnoreCase) &&
           !register.Contains("mặt trước", StringComparison.OrdinalIgnoreCase) &&
           !register.Contains("mặt sau", StringComparison.OrdinalIgnoreCase),
        "Register page must not include upload or QR/CCCD image controls.");

    Assert(controller.Contains("[HttpGet(\"pending-approval\")]", StringComparison.Ordinal) &&
           controller.Contains("PendingApproval", StringComparison.Ordinal),
        "Account controller must expose /account/pending-approval.");
    Assert(controller.Contains("t.TenDangNhap == loginKey || t.Email == loginKey", StringComparison.Ordinal),
        "Login action must authenticate by username or email.");

    foreach (var token in new[] { "--auth-primary", "--auth-primary-dark", "--auth-accent", "--auth-bg", "--auth-border", "--auth-text" })
    {
        Assert(authTheme.Contains(token, StringComparison.Ordinal), $"auth-theme.css must define {token}.");
    }

    Assert(authPages.Contains(".auth-login-card", StringComparison.Ordinal), "auth-pages.css must style the login card.");
    Assert(authPages.Contains(".auth-register-shell", StringComparison.Ordinal), "auth-pages.css must style the register shell.");
    Assert(authPages.Contains("@media", StringComparison.Ordinal), "auth-pages.css must include responsive rules.");
    Assert(authJs.Contains("data-auth-account-type", StringComparison.Ordinal), "auth-interactions.js must handle account type switching.");
    Assert(authJs.Contains("data-auth-password-toggle", StringComparison.Ordinal), "auth-interactions.js must handle password toggles.");
}

static void VerifyGreenThemeTokens()
{
    var siteCss = ReadRepoFile("wwwroot/css/site.css");
    var requiredTokens = new Dictionary<string, string>
    {
        ["--primary"] = "#2f8f1f",
        ["--primary-dark"] = "#1f6f12",
        ["--primary-darker"] = "#15530d",
        ["--primary-soft"] = "#eaf7e6",
        ["--primary-soft-2"] = "#f4fbf2",
        ["--accent"] = "#f2a100",
        ["--accent-dark"] = "#d48806",
        ["--accent-soft"] = "#fff4db",
        ["--bg"] = "#f6f8f7",
        ["--card"] = "#ffffff",
        ["--border"] = "#d8e3d5",
        ["--text"] = "#1f2937",
        ["--muted"] = "#6b7280",
        ["--success"] = "#2e7d32",
        ["--warning"] = "#f59e0b",
        ["--danger"] = "#dc2626",
        ["--info"] = "#2f8f1f"
    };

    foreach (var (token, value) in requiredTokens)
    {
        Assert(siteCss.Contains($"{token}: {value};", StringComparison.OrdinalIgnoreCase),
            $"site.css must define {token}: {value};");
    }

    foreach (var cssFile in RequiredThemeCssFiles())
    {
        var content = ReadRepoFile(cssFile);
        Assert(content.Contains("var(--primary", StringComparison.Ordinal) ||
               content.Contains("#2f8f1f", StringComparison.OrdinalIgnoreCase) ||
               content.Contains("#1f6f12", StringComparison.OrdinalIgnoreCase) ||
               content.Contains("#15530d", StringComparison.OrdinalIgnoreCase),
            $"{cssFile} must use the shared green theme palette.");
    }
}

static void VerifyNoLegacyBlueThemeValues()
{
    var legacyValues = new[]
    {
        "#074580",
        "#053360",
        "#1e64a8",
        "#0d47a1",
        "#08306b",
        "#eaf2ff",
        "#0d6efd",
        "#2563eb",
        "#1d4ed8",
        "#0369a1",
        "#06b6d4",
        "#032140"
    };

    foreach (var file in EnumerateThemeFiles())
    {
        var content = File.ReadAllText(file);
        foreach (var legacyValue in legacyValues)
        {
            Assert(!content.Contains(legacyValue, StringComparison.OrdinalIgnoreCase),
                $"{Path.GetRelativePath(GetProjectRoot(), file)} must not contain legacy blue value {legacyValue}.");
        }
    }
}

static void VerifyNoLegacyBootstrapBrandClasses()
{
    var legacyClasses = new[]
    {
        "bg-dark",
        "btn-dark",
        "table-dark",
        "bg-info",
        "btn-info"
    };

    var viewsRoot = Path.Combine(GetProjectRoot(), "Views");
    foreach (var file in Directory.EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories))
    {
        var content = File.ReadAllText(file);
        foreach (var legacyClass in legacyClasses)
        {
            Assert(!content.Contains(legacyClass, StringComparison.Ordinal),
                $"{Path.GetRelativePath(GetProjectRoot(), file)} must not use legacy Bootstrap branding class {legacyClass}.");
        }
    }
}

static void AssertHasRequiredWardForeignKey(IModel model, string typeName, string displayName)
{
    var clrType = Type.GetType($"{typeName}, ManagementOfForeigners");
    Assert(clrType is not null, $"{displayName} entity type must exist.");

    var entity = model.FindEntityType(clrType!);
    Assert(entity is not null, $"{displayName} must be mapped by EF Core.");

    var property = entity!.FindProperty("MaPhuongXa");
    Assert(property is not null, $"{displayName} must contain MaPhuongXa.");
    Assert(property!.ClrType == typeof(int), $"{displayName}.MaPhuongXa must be int.");
    Assert(property.IsNullable == false, $"{displayName}.MaPhuongXa must be required.");

    var fk = entity.GetForeignKeys().SingleOrDefault(x =>
        x.Properties.Any(p => p.Name == "MaPhuongXa") &&
        x.PrincipalEntityType.ClrType.FullName == "ManagementOfForeigners.Models.Entities.PhuongXa");
    Assert(fk is not null, $"{displayName}.MaPhuongXa must reference PhuongXa.");
}

static void AssertMappedProperties(ApplicationDbContext context, string typeName, string displayName, string[] expectedProperties)
{
    var clrType = Type.GetType($"{typeName}, ManagementOfForeigners");
    Assert(clrType is not null, $"{displayName} entity type must exist.");

    var entity = context.Model.FindEntityType(clrType!);
    Assert(entity is not null, $"{displayName} must be mapped by EF Core.");

    var actualProperties = entity!.GetProperties()
        .Select(p => p.Name)
        .OrderBy(x => x)
        .ToArray();

    var expected = expectedProperties.OrderBy(x => x).ToArray();
    Assert(actualProperties.SequenceEqual(expected),
        $"{displayName} mapped properties must match ERD exactly. Expected [{string.Join(", ", expected)}], actual [{string.Join(", ", actualProperties)}].");
}

static IEnumerable<string> RequiredThemeCssFiles()
{
    yield return "wwwroot/css/site.css";
    yield return "wwwroot/css/landing.css";
    yield return "wwwroot/css/khai-bao-tam-tru.css";
    yield return "wwwroot/css/admin-layout.css";
    yield return "wwwroot/css/admin.css";
    yield return "wwwroot/css/police-layout.css";
}

static IEnumerable<string> EnumerateThemeFiles()
{
    var root = GetProjectRoot();
    var viewFiles = Directory.EnumerateFiles(Path.Combine(root, "Views"), "*.cshtml", SearchOption.AllDirectories);
    var cssFiles = Directory.EnumerateFiles(Path.Combine(root, "wwwroot", "css"), "*.css", SearchOption.AllDirectories)
        .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}lib{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    return viewFiles.Concat(cssFiles);
}

static string ReadRepoFile(string relativePath)
{
    return File.ReadAllText(Path.Combine(GetProjectRoot(), relativePath));
}

static string GetProjectRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "ManagementOfForeigners.csproj")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Could not locate ManagementOfForeigners.csproj.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
