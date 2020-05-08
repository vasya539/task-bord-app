using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class MigrationAsIsAndFixRolesAutoIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "StoryPoint",
                table: "Items",
                nullable: true);

            //migrationBuilder.DropColumn(
            //    name: "Id",
            //    table: "AppUserRoles");

            //migrationBuilder.AddColumn<int>(
            //    name: "Id",
            //    table: "AppUserRoles",
            //    nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AppUserRoles",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
                //.OldAnnotation("SqlServer:Identity", "1, 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoryPoint",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AppUserRoles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
