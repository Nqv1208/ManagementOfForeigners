using ManagementOfForeigners.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

var tests = new List<(string Name, Action Test)>
{
    ("ERD maps PhuongXa and ward-scoped foreign keys", VerifyWardSchema),
    ("Residence declarations require an accommodation facility", VerifyDeclarationFacilityRequired)
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

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
