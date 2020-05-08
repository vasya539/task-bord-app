using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class SprintChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sprints");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Sprints",
                type: "datetime2",
                nullable: true);
        }
    }
}
