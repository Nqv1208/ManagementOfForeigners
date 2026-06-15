using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementOfForeigners.Migrations
{
    /// <inheritdoc />
    public partial class MakeForeignerAccountNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NguoiNuocNgoai_MaTaiKhoan",
                table: "NguoiNuocNgoai");

            migrationBuilder.AlterColumn<string>(
                name: "MaTaiKhoan",
                table: "NguoiNuocNgoai",
                type: "char(9)",
                fixedLength: true,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(9)",
                oldFixedLength: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiNuocNgoai_MaTaiKhoan",
                table: "NguoiNuocNgoai",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NguoiNuocNgoai_MaTaiKhoan",
                table: "NguoiNuocNgoai");

            migrationBuilder.AlterColumn<string>(
                name: "MaTaiKhoan",
                table: "NguoiNuocNgoai",
                type: "char(9)",
                fixedLength: true,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(9)",
                oldFixedLength: true,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiNuocNgoai_MaTaiKhoan",
                table: "NguoiNuocNgoai",
                column: "MaTaiKhoan",
                unique: true);
        }
    }
}
