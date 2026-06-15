using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementOfForeigners.Migrations
{
    /// <inheritdoc />
    public partial class AddLodgingOwnerRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CoSoLuuTru",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "CoSoLuuTru",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiHinh",
                table: "CoSoLuuTru",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSoKinhDoanh",
                table: "CoSoLuuTru",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoPhong",
                table: "CoSoLuuTru",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SucChuaToiDa",
                table: "CoSoLuuTru",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CoSoLuuTru",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "LoaiHinh",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "MaSoKinhDoanh",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "SoPhong",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "SucChuaToiDa",
                table: "CoSoLuuTru");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CoSoLuuTru");
        }
    }
}
