using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class WADCommentsV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "workApplicationData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "workApplicationData");
        }
    }
}
