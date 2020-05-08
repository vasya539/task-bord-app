using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class UserInfoAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectsUsers",
                table: "ProjectsUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectsUsers_UserId",
                table: "ProjectsUsers");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Sprints");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Sprints",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Sprints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sprints",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "UserId",
            //    table: "Projects",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "AspNetUsers",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectsUsers",
                table: "ProjectsUsers",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUsers_ProjectId",
                table: "ProjectsUsers",
                column: "ProjectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Projects_UserId",
            //    table: "Projects",
            //    column: "UserId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Projects_AspNetUsers_UserId",
            //    table: "Projects",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectsUsers",
                table: "ProjectsUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectsUsers_ProjectId",
                table: "ProjectsUsers");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Sprints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectsUsers",
                table: "ProjectsUsers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUsers_UserId",
                table: "ProjectsUsers",
                column: "UserId");
        }
    }
}
