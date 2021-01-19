using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class UseerRetryCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserRetryCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRetryCount",
                table: "Users");
        }
    }
}
