using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementOfForeigners.Migrations
{
    /// <inheritdoc />
    public partial class InitialErdSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhuongXa",
                columns: table => new
                {
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false),
                    TenPhuongXa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongXa", x => x.MaPhuongXa);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTaVaiTro = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "QuyenHan",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTaQuyen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenHan", x => x.MaQuyen);
                    table.ForeignKey(
                        name: "FK_QuyenHan_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CanBo",
                columns: table => new
                {
                    MaCanBo = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoCCCD = table.Column<string>(type: "char(12)", fixedLength: true, nullable: false),
                    NgayCapCCCD = table.Column<DateTime>(type: "date", nullable: false),
                    NoiCapCCCD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChiThuongTru = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DonViCongTac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapQuanLy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanBo", x => x.MaCanBo);
                    table.ForeignKey(
                        name: "FK_CanBo_PhuongXa_MaPhuongXa",
                        column: x => x.MaPhuongXa,
                        principalTable: "PhuongXa",
                        principalColumn: "MaPhuongXa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanBo_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChuCoSoLuuTru",
                columns: table => new
                {
                    MaChuCoSo = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoCCCD = table.Column<string>(type: "char(12)", fixedLength: true, nullable: false),
                    NgayCapCCCD = table.Column<DateTime>(type: "date", nullable: false),
                    NoiCapCCCD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChiThuongTru = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuCoSoLuuTru", x => x.MaChuCoSo);
                    table.ForeignKey(
                        name: "FK_ChuCoSoLuuTru_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuCapNhatThongTinCaNhan",
                columns: table => new
                {
                    MaLSCapNhat = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    TruongCapNhat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTriCu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GiaTriMoi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    LyDoCapNhat = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuCapNhatThongTinCaNhan", x => x.MaLSCapNhat);
                    table.ForeignKey(
                        name: "FK_LichSuCapNhatThongTinCaNhan_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiNuocNgoai",
                columns: table => new
                {
                    MaNguoiNuocNgoai = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    QuocTich = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoHoChieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayCapHoChieu = table.Column<DateTime>(type: "date", nullable: false),
                    NgayHetHanHoChieu = table.Column<DateTime>(type: "date", nullable: false),
                    LoaiVisa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayHetHanVisa = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiNuocNgoai", x => x.MaNguoiNuocNgoai);
                    table.ForeignKey(
                        name: "FK_NguoiNuocNgoai_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoSoLuuTru",
                columns: table => new
                {
                    MaCoSoLuuTru = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaPhuongXa = table.Column<int>(type: "int", nullable: false),
                    MaChuCoSo = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    TenCoSo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoSoLuuTru", x => x.MaCoSoLuuTru);
                    table.ForeignKey(
                        name: "FK_CoSoLuuTru_ChuCoSoLuuTru_MaChuCoSo",
                        column: x => x.MaChuCoSo,
                        principalTable: "ChuCoSoLuuTru",
                        principalColumn: "MaChuCoSo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoSoLuuTru_PhuongXa_MaPhuongXa",
                        column: x => x.MaPhuongXa,
                        principalTable: "PhuongXa",
                        principalColumn: "MaPhuongXa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaoCaoViPham",
                columns: table => new
                {
                    MaBaoCao = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaNguoiNuocNgoai = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaCanBo = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    NoiDungBaoCao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    TrangThaiXuLy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoViPham", x => x.MaBaoCao);
                    table.ForeignKey(
                        name: "FK_BaoCaoViPham_CanBo_MaCanBo",
                        column: x => x.MaCanBo,
                        principalTable: "CanBo",
                        principalColumn: "MaCanBo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaoCaoViPham_NguoiNuocNgoai_MaNguoiNuocNgoai",
                        column: x => x.MaNguoiNuocNgoai,
                        principalTable: "NguoiNuocNgoai",
                        principalColumn: "MaNguoiNuocNgoai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CanhBaoViPham",
                columns: table => new
                {
                    MaCanhBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiNuocNgoai = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaCanBo = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    LoaiViPham = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDungCanhBao = table.Column<string>(type: "text", nullable: false),
                    MucDoViPham = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayCanhBao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanhBaoViPham", x => x.MaCanhBao);
                    table.ForeignKey(
                        name: "FK_CanhBaoViPham_CanBo_MaCanBo",
                        column: x => x.MaCanBo,
                        principalTable: "CanBo",
                        principalColumn: "MaCanBo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanhBaoViPham_NguoiNuocNgoai_MaNguoiNuocNgoai",
                        column: x => x.MaNguoiNuocNgoai,
                        principalTable: "NguoiNuocNgoai",
                        principalColumn: "MaNguoiNuocNgoai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoKhaiBaoTamTru",
                columns: table => new
                {
                    MaHSKhaiBao = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaCoSoLuuTru = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    NgayKhaiBao = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayBatDau = table.Column<DateTime>(type: "date", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "date", nullable: false),
                    MucDichLuuTru = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChiLuuTru = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoKhaiBaoTamTru", x => x.MaHSKhaiBao);
                    table.ForeignKey(
                        name: "FK_HoSoKhaiBaoTamTru_CoSoLuuTru_MaCoSoLuuTru",
                        column: x => x.MaCoSoLuuTru,
                        principalTable: "CoSoLuuTru",
                        principalColumn: "MaCoSoLuuTru",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoKhaiBaoTamTru_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuCuTru",
                columns: table => new
                {
                    MaLSLuuTru = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaNguoiNuocNgoai = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    MaCoSoLuuTru = table.Column<string>(type: "char(9)", fixedLength: true, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: true),
                    Phong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuCuTru", x => x.MaLSLuuTru);
                    table.ForeignKey(
                        name: "FK_LichSuCuTru_CoSoLuuTru_MaCoSoLuuTru",
                        column: x => x.MaCoSoLuuTru,
                        principalTable: "CoSoLuuTru",
                        principalColumn: "MaCoSoLuuTru",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuCuTru_NguoiNuocNgoai_MaNguoiNuocNgoai",
                        column: x => x.MaNguoiNuocNgoai,
                        principalTable: "NguoiNuocNgoai",
                        principalColumn: "MaNguoiNuocNgoai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoViPham_MaCanBo",
                table: "BaoCaoViPham",
                column: "MaCanBo");

            migrationBuilder.CreateIndex(
                name: "IX_BaoCaoViPham_MaNguoiNuocNgoai",
                table: "BaoCaoViPham",
                column: "MaNguoiNuocNgoai");

            migrationBuilder.CreateIndex(
                name: "IX_CanBo_MaPhuongXa",
                table: "CanBo",
                column: "MaPhuongXa");

            migrationBuilder.CreateIndex(
                name: "IX_CanBo_MaTaiKhoan",
                table: "CanBo",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanBo_SoCCCD",
                table: "CanBo",
                column: "SoCCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoViPham_MaCanBo",
                table: "CanhBaoViPham",
                column: "MaCanBo");

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaoViPham_MaNguoiNuocNgoai",
                table: "CanhBaoViPham",
                column: "MaNguoiNuocNgoai");

            migrationBuilder.CreateIndex(
                name: "IX_ChuCoSoLuuTru_MaTaiKhoan",
                table: "ChuCoSoLuuTru",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChuCoSoLuuTru_SoCCCD",
                table: "ChuCoSoLuuTru",
                column: "SoCCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoSoLuuTru_MaChuCoSo",
                table: "CoSoLuuTru",
                column: "MaChuCoSo");

            migrationBuilder.CreateIndex(
                name: "IX_CoSoLuuTru_MaPhuongXa",
                table: "CoSoLuuTru",
                column: "MaPhuongXa");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoKhaiBaoTamTru_MaCoSoLuuTru",
                table: "HoSoKhaiBaoTamTru",
                column: "MaCoSoLuuTru");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoKhaiBaoTamTru_MaTaiKhoan",
                table: "HoSoKhaiBaoTamTru",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuCapNhatThongTinCaNhan_MaTaiKhoan",
                table: "LichSuCapNhatThongTinCaNhan",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuCuTru_MaCoSoLuuTru",
                table: "LichSuCuTru",
                column: "MaCoSoLuuTru");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuCuTru_MaNguoiNuocNgoai",
                table: "LichSuCuTru",
                column: "MaNguoiNuocNgoai");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiNuocNgoai_MaTaiKhoan",
                table: "NguoiNuocNgoai",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiNuocNgoai_SoHoChieu",
                table: "NguoiNuocNgoai",
                column: "SoHoChieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuyenHan_MaVaiTro",
                table: "QuyenHan",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaVaiTro",
                table: "TaiKhoan",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCaoViPham");

            migrationBuilder.DropTable(
                name: "CanhBaoViPham");

            migrationBuilder.DropTable(
                name: "HoSoKhaiBaoTamTru");

            migrationBuilder.DropTable(
                name: "LichSuCapNhatThongTinCaNhan");

            migrationBuilder.DropTable(
                name: "LichSuCuTru");

            migrationBuilder.DropTable(
                name: "QuyenHan");

            migrationBuilder.DropTable(
                name: "CanBo");

            migrationBuilder.DropTable(
                name: "CoSoLuuTru");

            migrationBuilder.DropTable(
                name: "NguoiNuocNgoai");

            migrationBuilder.DropTable(
                name: "ChuCoSoLuuTru");

            migrationBuilder.DropTable(
                name: "PhuongXa");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
