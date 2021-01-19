using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class BoardMembersFIxV6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardMembersInitialCount",
                table: "workApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardMembersInitialCount",
                table: "workApplicationData");
        }
    }
}
