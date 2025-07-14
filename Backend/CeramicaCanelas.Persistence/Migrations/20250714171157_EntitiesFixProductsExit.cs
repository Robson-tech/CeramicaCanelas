using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeramicaCanelas.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesFixProductsExit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductExits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExits_UserId",
                table: "ProductExits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExits_AspNetUsers_UserId",
                table: "ProductExits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductExits_AspNetUsers_UserId",
                table: "ProductExits");

            migrationBuilder.DropIndex(
                name: "IX_ProductExits_UserId",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductExits");
        }
    }
}
