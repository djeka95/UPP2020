using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class BoardMembersFIx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardMembersPositiveReviewCount",
                table: "workApplicationData");

            migrationBuilder.DropColumn(
                name: "BoardMembersReviewCount",
                table: "workApplicationData");

            migrationBuilder.AddColumn<bool>(
                name: "BoardMembeNeedsMoreData",
                table: "workApplicationData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BoardMembersDeclined",
                table: "workApplicationData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardMembeNeedsMoreData",
                table: "workApplicationData");

            migrationBuilder.DropColumn(
                name: "BoardMembersDeclined",
                table: "workApplicationData");

            migrationBuilder.AddColumn<int>(
                name: "BoardMembersPositiveReviewCount",
                table: "workApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoardMembersReviewCount",
                table: "workApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
