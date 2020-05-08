using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class NewTableForAvatars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarTail",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Avatars",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    AvatarTail = table.Column<int>(nullable: true),
                    Avatar = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatars", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Avatars_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avatars");

            migrationBuilder.AddColumn<int>(
                name: "AvatarTail",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
