using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class BoardMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkApplicationDataId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "workApplicationData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardMembersReviewCount = table.Column<int>(type: "int", nullable: false),
                    BoardMembersPositiveReviewCount = table.Column<int>(type: "int", nullable: false),
                    processDefinitionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    processInstanceId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workApplicationData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WorkApplicationDataId",
                table: "Users",
                column: "WorkApplicationDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_workApplicationData_WorkApplicationDataId",
                table: "Users",
                column: "WorkApplicationDataId",
                principalTable: "workApplicationData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_workApplicationData_WorkApplicationDataId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "workApplicationData");

            migrationBuilder.DropIndex(
                name: "IX_Users_WorkApplicationDataId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkApplicationDataId",
                table: "Users");
        }
    }
}
