using Microsoft.EntityFrameworkCore.Migrations;

namespace lit_udr.Migrations
{
    public partial class BoardMembersFIxV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_workApplicationData_WorkApplicationDataId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_WorkApplicationDataId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkApplicationDataId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserReview",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkApplicationDataId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReview", x => new { x.UserId, x.WorkApplicationDataId });
                    table.ForeignKey(
                        name: "FK_UserReview_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReview_workApplicationData_WorkApplicationDataId",
                        column: x => x.WorkApplicationDataId,
                        principalTable: "workApplicationData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserReview_WorkApplicationDataId",
                table: "UserReview",
                column: "WorkApplicationDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReview");

            migrationBuilder.AddColumn<int>(
                name: "WorkApplicationDataId",
                table: "Users",
                type: "int",
                nullable: true);

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
    }
}
