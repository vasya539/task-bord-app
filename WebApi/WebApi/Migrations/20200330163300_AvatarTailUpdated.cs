using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class AvatarTailUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarTail",
                table: "Avatars");

            migrationBuilder.AddColumn<int>(
                name: "AvatarTail",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarTail",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "AvatarTail",
                table: "Avatars",
                type: "int",
                nullable: true);
        }
    }
}
