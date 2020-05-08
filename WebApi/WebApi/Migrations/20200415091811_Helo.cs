using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Helo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemsRelations",
                columns: table => new
                {
                    FirstItemId = table.Column<int>(nullable: false),
                    SecondItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsRelations", x => new { x.FirstItemId, x.SecondItemId });
                    table.ForeignKey(
                        name: "FK_ItemsRelations_Items_FirstItemId",
                        column: x => x.FirstItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemsRelations_Items_SecondItemId",
                        column: x => x.SecondItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsRelations_SecondItemId",
                table: "ItemsRelations",
                column: "SecondItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsRelations");
        }
    }
}
