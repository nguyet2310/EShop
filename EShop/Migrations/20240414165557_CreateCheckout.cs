using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.Migrations
{
    public partial class CreateCheckout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
