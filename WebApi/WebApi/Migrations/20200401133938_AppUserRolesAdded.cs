using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class AppUserRolesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),//.Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUsers_UserRole",
                table: "ProjectsUsers",
                column: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectsUsers_AppUserRoles_UserRole",
                table: "ProjectsUsers",
                column: "UserRole",
                principalTable: "AppUserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectsUsers_AppUserRoles_UserRole",
                table: "ProjectsUsers");

            migrationBuilder.DropTable(
                name: "AppUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_ProjectsUsers_UserRole",
                table: "ProjectsUsers");
        }
    }
}
