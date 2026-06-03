using Microsoft.EntityFrameworkCore;
using ManagementOfForeigners.Models.Entities;

namespace ManagementOfForeigners.Data;

/// <summary>
/// DbContext chính cho hệ thống quản lý người nước ngoài
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<PhanQuyenVaiTro> PhanQuyenVaiTros => Set<PhanQuyenVaiTro>();
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<CanBo> CanBos => Set<CanBo>();
    public DbSet<ChuCoSoLuuTru> ChuCoSoLuuTrus => Set<ChuCoSoLuuTru>();
    public DbSet<NguoiNuocNgoai> NguoiNuocNgoais => Set<NguoiNuocNgoai>();
    public DbSet<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus => Set<HoSoKhaiBaoTamTru>();
    public DbSet<CoSoLuuTru> CoSoLuuTrus => Set<CoSoLuuTru>();
    public DbSet<LichSuCuTru> LichSuCuTrus => Set<LichSuCuTru>();
    public DbSet<LichSuLuuTru> LichSuLuuTrus => Set<LichSuLuuTru>();
    public DbSet<LichSuCapNhatThongTin> LichSuCapNhatThongTins => Set<LichSuCapNhatThongTin>();
    public DbSet<CanhBaoViPham> CanhBaoViPhams => Set<CanhBaoViPham>();
    public DbSet<BaoCaoViPham> BaoCaoViPhams => Set<BaoCaoViPham>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== VaiTro =====
        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro);
            entity.Property(e => e.MaVaiTro).ValueGeneratedNever();
            entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");
        });

        // ===== PhanQuyenVaiTro =====
        modelBuilder.Entity<PhanQuyenVaiTro>(entity =>
        {
            entity.HasKey(e => e.MaQuyen);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.VaiTro)
                  .WithMany(v => v.PhanQuyenVaiTros)
                  .HasForeignKey(e => e.MaVaiTro)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== TaiKhoan =====
        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan);
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();

            entity.HasIndex(e => e.TenDangNhap).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");

            entity.HasOne(e => e.VaiTro)
                  .WithMany(v => v.TaiKhoans)
                  .HasForeignKey(e => e.MaVaiTro)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== CanBo =====
        modelBuilder.Entity<CanBo>(entity =>
        {
            entity.HasKey(e => e.MaCanBo);
            entity.Property(e => e.MaCanBo).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();
            entity.Property(e => e.SoCCCD).IsFixedLength();

            entity.HasIndex(e => e.MaTaiKhoan).IsUnique();
            entity.HasIndex(e => e.SoCCCD).IsUnique();

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.CanBo)
                  .HasForeignKey<CanBo>(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== ChuCoSoLuuTru =====
        modelBuilder.Entity<ChuCoSoLuuTru>(entity =>
        {
            entity.HasKey(e => e.MaChuCoSo);
            entity.Property(e => e.MaChuCoSo).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();
            entity.Property(e => e.SoCCCD).IsFixedLength();

            entity.HasIndex(e => e.MaTaiKhoan).IsUnique();
            entity.HasIndex(e => e.SoCCCD).IsUnique();

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.ChuCoSoLuuTru)
                  .HasForeignKey<ChuCoSoLuuTru>(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== NguoiNuocNgoai =====
        modelBuilder.Entity<NguoiNuocNgoai>(entity =>
        {
            entity.HasKey(e => e.MaNguoiNuocNgoai);
            entity.Property(e => e.MaNguoiNuocNgoai).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();

            entity.HasIndex(e => e.MaTaiKhoan).IsUnique();
            entity.HasIndex(e => e.SoHoChieu).IsUnique();

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.NguoiNuocNgoai)
                  .HasForeignKey<NguoiNuocNgoai>(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);

            // Index cho tìm kiếm
            entity.HasIndex(e => e.HoTen);
            entity.HasIndex(e => e.QuocTich);
        });

        // ===== HoSoKhaiBaoTamTru =====
        modelBuilder.Entity<HoSoKhaiBaoTamTru>(entity =>
        {
            entity.HasKey(e => e.MaHSKhaiBao);
            entity.Property(e => e.MaHSKhaiBao).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();

            entity.Property(e => e.NgayKhaiBao).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ duyệt");

            entity.HasOne(e => e.TaiKhoan)
                  .WithMany(t => t.HoSoKhaiBaoTamTrus)
                  .HasForeignKey(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CoSoLuuTru)
                  .WithMany(c => c.HoSoKhaiBaoTamTrus)
                  .HasForeignKey(e => e.MaCoSoLuuTru)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);

            // Index cho tra cứu
            entity.HasIndex(e => e.TrangThai);
            entity.HasIndex(e => e.NgayKhaiBao);
        });

        // ===== CoSoLuuTru =====
        modelBuilder.Entity<CoSoLuuTru>(entity =>
        {
            entity.HasKey(e => e.MaCoSoLuuTru);
            entity.Property(e => e.MaCoSoLuuTru).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();

            entity.Property(e => e.TrangThai).HasDefaultValue("Đang hoạt động");

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.CoSoLuuTru)
                  .HasForeignKey<CoSoLuuTru>(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.TenCoSo);
        });

        // ===== LichSuCuTru =====
        modelBuilder.Entity<LichSuCuTru>(entity =>
        {
            entity.HasKey(e => e.MaLuuTru);
            entity.Property(e => e.MaLuuTru).IsFixedLength();
            entity.Property(e => e.MaNguoiNuocNgoai).IsFixedLength();

            entity.HasOne(e => e.NguoiNuocNgoai)
                  .WithMany(n => n.LichSuCuTrus)
                  .HasForeignKey(e => e.MaNguoiNuocNgoai)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CoSoLuuTru)
                  .WithMany(c => c.LichSuCuTrus)
                  .HasForeignKey(e => e.MaCoSoLuuTru)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);
        });

        // ===== LichSuLuuTru =====
        modelBuilder.Entity<LichSuLuuTru>(entity =>
        {
            entity.HasKey(e => e.MaLSLuuTru);
            entity.Property(e => e.MaLSLuuTru).IsFixedLength();
            entity.Property(e => e.MaNguoiNuocNgoai).IsFixedLength();
            entity.Property(e => e.MaCoSoLuuTru).IsFixedLength();

            entity.HasOne(e => e.NguoiNuocNgoai)
                  .WithMany(n => n.LichSuLuuTrus)
                  .HasForeignKey(e => e.MaNguoiNuocNgoai)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CoSoLuuTru)
                  .WithMany(c => c.LichSuLuuTrus)
                  .HasForeignKey(e => e.MaCoSoLuuTru)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.TrangThai);
        });

        // ===== LichSuCapNhatThongTin =====
        modelBuilder.Entity<LichSuCapNhatThongTin>(entity =>
        {
            entity.HasKey(e => e.MaLSCapNhat);
            entity.Property(e => e.MaLSCapNhat).IsFixedLength();
            entity.Property(e => e.MaTaiKhoan).IsFixedLength();

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.TaiKhoan)
                  .WithMany(t => t.LichSuCapNhats)
                  .HasForeignKey(e => e.MaTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== CanhBaoViPham =====
        modelBuilder.Entity<CanhBaoViPham>(entity =>
        {
            entity.HasKey(e => e.MaCanhBao);

            entity.Property(e => e.MaNguoiNuocNgoai).IsFixedLength();
            entity.Property(e => e.MaCanBo).IsFixedLength();
            entity.Property(e => e.NgayCanhBao).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.TrangThai).HasDefaultValue("Đã gửi");

            entity.HasOne(e => e.NguoiNuocNgoai)
                  .WithMany(n => n.CanhBaoViPhams)
                  .HasForeignKey(e => e.MaNguoiNuocNgoai)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CanBo)
                  .WithMany()
                  .HasForeignKey(e => e.MaCanBo)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.MucDoViPham);
            entity.HasIndex(e => e.TrangThai);
        });

        // ===== BaoCaoViPham =====
        modelBuilder.Entity<BaoCaoViPham>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao);
            entity.Property(e => e.MaBaoCao).IsFixedLength();
            entity.Property(e => e.MaNguoiNuocNgoai).IsFixedLength();
            entity.Property(e => e.MaCanBo).IsFixedLength();

            entity.Property(e => e.TrangThaiXuLy).HasDefaultValue("Chưa xử lý");

            entity.HasOne(e => e.NguoiNuocNgoai)
                  .WithMany(n => n.BaoCaoViPhams)
                  .HasForeignKey(e => e.MaNguoiNuocNgoai)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CanBo)
                  .WithMany()
                  .HasForeignKey(e => e.MaCanBo)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.TrangThaiXuLy);
        });
    }
}
