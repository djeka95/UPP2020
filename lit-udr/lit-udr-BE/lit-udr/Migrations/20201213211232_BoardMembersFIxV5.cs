using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class BoardMembersFIxV5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardMembersDeclined",
                table: "workApplicationData");

            migrationBuilder.AddColumn<int>(
                name: "BoardMembersApprove",
                table: "workApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardMembersApprove",
                table: "workApplicationData");

            migrationBuilder.AddColumn<bool>(
                name: "BoardMembersDeclined",
                table: "workApplicationData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
