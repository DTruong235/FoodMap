using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodMap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVaiTroToKhachhang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VaiTro",
                table: "Khachhang",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VaiTro",
                table: "Khachhang");
        }
    }
}
