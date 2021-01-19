using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class NewUserDataFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewUserEmmail",
                table: "NewUserData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewUserEmmail",
                table: "NewUserData");
        }
    }
}
