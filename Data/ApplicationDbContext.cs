using ManagementOfForeigners.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfForeigners.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PhuongXa> PhuongXas => Set<PhuongXa>();
    public DbSet<QuyenHan> QuyenHans => Set<QuyenHan>();
    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LichSuCapNhatThongTinCaNhan> LichSuCapNhatThongTinCaNhans => Set<LichSuCapNhatThongTinCaNhan>();
    public DbSet<CanBo> CanBos => Set<CanBo>();
    public DbSet<NguoiNuocNgoai> NguoiNuocNgoais => Set<NguoiNuocNgoai>();
    public DbSet<ChuCoSoLuuTru> ChuCoSoLuuTrus => Set<ChuCoSoLuuTru>();
    public DbSet<CoSoLuuTru> CoSoLuuTrus => Set<CoSoLuuTru>();
    public DbSet<HoSoKhaiBaoTamTru> HoSoKhaiBaoTamTrus => Set<HoSoKhaiBaoTamTru>();
    public DbSet<LichSuCuTru> LichSuCuTrus => Set<LichSuCuTru>();
    public DbSet<CanhBaoViPham> CanhBaoViPhams => Set<CanhBaoViPham>();
    public DbSet<BaoCaoViPham> BaoCaoViPhams => Set<BaoCaoViPham>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PhuongXa>(e =>
        {
            e.ToTable("PhuongXa");
            e.HasKey(x => x.MaPhuongXa);
            e.Property(x => x.MaPhuongXa).ValueGeneratedNever();
            e.Property(x => x.TenPhuongXa).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<VaiTro>(e =>
        {
            e.ToTable("VaiTro");
            e.HasKey(x => x.MaVaiTro);
            e.Property(x => x.MaVaiTro).ValueGeneratedNever();
            e.Property(x => x.TenVaiTro).HasMaxLength(50).IsRequired();
            e.Property(x => x.MoTaVaiTro).HasMaxLength(255);
            e.Property(x => x.NgayTao).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<QuyenHan>(e =>
        {
            e.ToTable("QuyenHan");
            e.HasKey(x => x.MaQuyen);
            e.Property(x => x.MaQuyen).ValueGeneratedOnAdd();
            e.Property(x => x.TenQuyen).HasMaxLength(100).IsRequired();
            e.Property(x => x.MoTaQuyen).HasMaxLength(255);
            e.Property(x => x.NgayCapNhat).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.HasOne(x => x.VaiTro)
                .WithMany(v => v.QuyenHans)
                .HasForeignKey(x => x.MaVaiTro)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaiKhoan>(e =>
        {
            e.ToTable("TaiKhoan");
            e.HasKey(x => x.MaTaiKhoan);
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.TenDangNhap).HasMaxLength(50).IsRequired();
            e.Property(x => x.MatKhauHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.SoDienThoai).HasMaxLength(15);
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
            e.Property(x => x.NgayTao).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.LanDangNhapCuoi).HasColumnType("datetime");
            e.HasIndex(x => x.TenDangNhap).IsUnique();
            e.HasOne(x => x.VaiTro)
                .WithMany(v => v.TaiKhoans)
                .HasForeignKey(x => x.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LichSuCapNhatThongTinCaNhan>(e =>
        {
            e.ToTable("LichSuCapNhatThongTinCaNhan");
            e.HasKey(x => x.MaLSCapNhat);
            e.Property(x => x.MaLSCapNhat).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.TruongCapNhat).HasMaxLength(100).IsRequired();
            e.Property(x => x.GiaTriCu).HasMaxLength(255);
            e.Property(x => x.GiaTriMoi).HasMaxLength(255).IsRequired();
            e.Property(x => x.NgayCapNhat).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.LyDoCapNhat).HasMaxLength(255);
            e.Property(x => x.TrangThai).HasMaxLength(25);
            e.HasOne(x => x.TaiKhoan)
                .WithMany(t => t.LichSuCapNhats)
                .HasForeignKey(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CanBo>(e =>
        {
            e.ToTable("CanBo");
            e.HasKey(x => x.MaCanBo);
            e.Property(x => x.MaCanBo).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.SoCCCD).HasColumnType("char(12)").IsFixedLength().IsRequired();
            e.Property(x => x.NgayCapCCCD).HasColumnType("date");
            e.Property(x => x.NoiCapCCCD).HasMaxLength(100);
            e.Property(x => x.DiaChiThuongTru).HasMaxLength(255);
            e.Property(x => x.NgaySinh).HasColumnType("date");
            e.Property(x => x.GioiTinh).HasMaxLength(10).IsRequired();
            e.Property(x => x.DonViCongTac).HasMaxLength(100);
            e.Property(x => x.ChucVu).HasMaxLength(50);
            e.Property(x => x.CapQuanLy).HasMaxLength(50);
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.SoCCCD).IsUnique();
            e.HasIndex(x => x.MaTaiKhoan).IsUnique();
            e.HasOne(x => x.TaiKhoan)
                .WithOne(t => t.CanBo)
                .HasForeignKey<CanBo>(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PhuongXa)
                .WithMany(p => p.CanBos)
                .HasForeignKey(x => x.MaPhuongXa)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NguoiNuocNgoai>(e =>
        {
            e.ToTable("NguoiNuocNgoai");
            e.HasKey(x => x.MaNguoiNuocNgoai);
            e.Property(x => x.MaNguoiNuocNgoai).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength().IsRequired(false);
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.NgaySinh).HasColumnType("date");
            e.Property(x => x.GioiTinh).HasMaxLength(10).IsRequired();
            e.Property(x => x.QuocTich).HasMaxLength(50).IsRequired();
            e.Property(x => x.SoHoChieu).HasMaxLength(20).IsRequired();
            e.Property(x => x.NgayCapHoChieu).HasColumnType("date");
            e.Property(x => x.NgayHetHanHoChieu).HasColumnType("date");
            e.Property(x => x.LoaiVisa).HasMaxLength(20).IsRequired();
            e.Property(x => x.NgayHetHanVisa).HasColumnType("date");
            e.HasIndex(x => x.SoHoChieu).IsUnique();
            e.HasIndex(x => x.MaTaiKhoan).IsUnique().HasFilter("[MaTaiKhoan] IS NOT NULL");
            e.HasOne(x => x.TaiKhoan)
                .WithOne(t => t.NguoiNuocNgoai)
                .HasForeignKey<NguoiNuocNgoai>(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChuCoSoLuuTru>(e =>
        {
            e.ToTable("ChuCoSoLuuTru");
            e.HasKey(x => x.MaChuCoSo);
            e.Property(x => x.MaChuCoSo).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.NgaySinh).HasColumnType("date");
            e.Property(x => x.GioiTinh).HasMaxLength(10).IsRequired();
            e.Property(x => x.SoCCCD).HasColumnType("char(12)").IsFixedLength().IsRequired();
            e.Property(x => x.NgayCapCCCD).HasColumnType("date");
            e.Property(x => x.NoiCapCCCD).HasMaxLength(100);
            e.Property(x => x.DiaChiThuongTru).HasMaxLength(255);
            e.HasIndex(x => x.SoCCCD).IsUnique();
            e.HasIndex(x => x.MaTaiKhoan).IsUnique();
            e.HasOne(x => x.TaiKhoan)
                .WithOne(t => t.ChuCoSoLuuTru)
                .HasForeignKey<ChuCoSoLuuTru>(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CoSoLuuTru>(e =>
        {
            e.ToTable("CoSoLuuTru");
            e.HasKey(x => x.MaCoSoLuuTru);
            e.Property(x => x.MaCoSoLuuTru).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaChuCoSo).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.TenCoSo).HasMaxLength(255).IsRequired();
            e.Property(x => x.DiaChi).HasMaxLength(255).IsRequired();
            e.Property(x => x.SoDienThoai).HasMaxLength(15).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.ChuCoSoLuuTru)
                .WithMany(c => c.CoSoLuuTrus)
                .HasForeignKey(x => x.MaChuCoSo)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PhuongXa)
                .WithMany(p => p.CoSoLuuTrus)
                .HasForeignKey(x => x.MaPhuongXa)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HoSoKhaiBaoTamTru>(e =>
        {
            e.ToTable("HoSoKhaiBaoTamTru");
            e.HasKey(x => x.MaHSKhaiBao);
            e.Property(x => x.MaHSKhaiBao).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaTaiKhoan).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaCoSoLuuTru).HasColumnType("char(9)").IsFixedLength().IsRequired();
            e.Property(x => x.NgayKhaiBao).HasColumnType("date").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.NgayBatDau).HasColumnType("date");
            e.Property(x => x.NgayKetThuc).HasColumnType("date");
            e.Property(x => x.MucDichLuuTru).HasMaxLength(100).IsRequired();
            e.Property(x => x.DiaChiLuuTru).HasMaxLength(255).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(25).IsRequired();
            e.Property(x => x.LyDoTuChoi).HasMaxLength(255);
            e.Property(x => x.GhiChu).HasMaxLength(255);
            e.HasOne(x => x.TaiKhoan)
                .WithMany(t => t.HoSoKhaiBaoTamTrus)
                .HasForeignKey(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CoSoLuuTru)
                .WithMany(c => c.HoSoKhaiBaoTamTrus)
                .HasForeignKey(x => x.MaCoSoLuuTru)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LichSuCuTru>(e =>
        {
            e.ToTable("LichSuCuTru");
            e.HasKey(x => x.MaLSLuuTru);
            e.Property(x => x.MaLSLuuTru).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaNguoiNuocNgoai).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaCoSoLuuTru).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.NgayBatDau).HasColumnType("datetime");
            e.Property(x => x.NgayKetThuc).HasColumnType("datetime");
            e.Property(x => x.Phong).HasMaxLength(20);
            e.Property(x => x.TrangThai).HasMaxLength(20);
            e.Property(x => x.GhiChu).HasMaxLength(255);
            e.HasOne(x => x.NguoiNuocNgoai)
                .WithMany(n => n.LichSuCuTrus)
                .HasForeignKey(x => x.MaNguoiNuocNgoai)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CoSoLuuTru)
                .WithMany(c => c.LichSuCuTrus)
                .HasForeignKey(x => x.MaCoSoLuuTru)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CanhBaoViPham>(e =>
        {
            e.ToTable("CanhBaoViPham");
            e.HasKey(x => x.MaCanhBao);
            e.Property(x => x.MaCanhBao).ValueGeneratedOnAdd();
            e.Property(x => x.MaNguoiNuocNgoai).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaCanBo).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.LoaiViPham).HasMaxLength(100).IsUnicode(true).IsRequired();
            e.Property(x => x.NoiDungCanhBao).HasColumnType("nvarchar(max)").IsUnicode(true).IsRequired();
            e.Property(x => x.MucDoViPham).HasMaxLength(20).IsRequired();
            e.Property(x => x.NgayCanhBao).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.TrangThai).HasMaxLength(20).IsRequired();
            e.Property(x => x.GhiChu).HasColumnType("nvarchar(max)").IsUnicode(true);
            e.HasOne(x => x.NguoiNuocNgoai)
                .WithMany(n => n.CanhBaoViPhams)
                .HasForeignKey(x => x.MaNguoiNuocNgoai)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CanBo)
                .WithMany(c => c.CanhBaoViPhams)
                .HasForeignKey(x => x.MaCanBo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BaoCaoViPham>(e =>
        {
            e.ToTable("BaoCaoViPham");
            e.HasKey(x => x.MaBaoCao);
            e.Property(x => x.MaBaoCao).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaNguoiNuocNgoai).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.MaCanBo).HasColumnType("char(9)").IsFixedLength();
            e.Property(x => x.NoiDungBaoCao).HasMaxLength(255).IsUnicode(true).IsRequired();
            e.Property(x => x.NgayBaoCao).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.TrangThaiXuLy).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.NguoiNuocNgoai)
                .WithMany(n => n.BaoCaoViPhams)
                .HasForeignKey(x => x.MaNguoiNuocNgoai)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CanBo)
                .WithMany(c => c.BaoCaoViPhams)
                .HasForeignKey(x => x.MaCanBo)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
