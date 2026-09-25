using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodMap.Data.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatThuocTinhNCC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "Nhacungcap",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "MaSoThue",
                table: "Nhacungcap",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatKhau",
                table: "Nhacungcap",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDangKy",
                table: "Nhacungcap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDuyet",
                table: "Nhacungcap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDaiDien",
                table: "Nhacungcap",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiXacThuc",
                table: "Nhacungcap",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaSoThue",
                table: "Nhacungcap");

            migrationBuilder.DropColumn(
                name: "MatKhau",
                table: "Nhacungcap");

            migrationBuilder.DropColumn(
                name: "NgayDangKy",
                table: "Nhacungcap");

            migrationBuilder.DropColumn(
                name: "NgayDuyet",
                table: "Nhacungcap");

            migrationBuilder.DropColumn(
                name: "NguoiDaiDien",
                table: "Nhacungcap");

            migrationBuilder.DropColumn(
                name: "TrangThaiXacThuc",
                table: "Nhacungcap");

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "Nhacungcap",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
