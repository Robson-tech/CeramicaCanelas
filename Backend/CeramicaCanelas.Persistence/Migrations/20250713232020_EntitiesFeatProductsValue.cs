using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeramicaCanelas.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesFeatProductsValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
