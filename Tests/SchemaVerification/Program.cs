using ManagementOfForeigners.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

var tests = new List<(string Name, Action Test)>
{
    ("ERD maps PhuongXa and ward-scoped foreign keys", VerifyWardSchema),
    ("Residence declarations require an accommodation facility", VerifyDeclarationFacilityRequired),
    ("Seed data contains the 94 new Da Nang administrative units", VerifyDaNangAdministrativeUnits),
    ("Large seed plan covers operational tables", VerifyLargeSeedTargets),
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
