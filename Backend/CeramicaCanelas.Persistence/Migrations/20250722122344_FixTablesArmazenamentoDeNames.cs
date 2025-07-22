using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeramicaCanelas.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTablesArmazenamentoDeNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "ProductExits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOperator",
                table: "ProductExits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameProduct",
                table: "ProductExits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOperator",
                table: "ProductEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameProduct",
                table: "ProductEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameSupplier",
                table: "ProductEntries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "NameOperator",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "NameProduct",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "NameOperator",
                table: "ProductEntries");

            migrationBuilder.DropColumn(
                name: "NameProduct",
                table: "ProductEntries");

            migrationBuilder.DropColumn(
                name: "NameSupplier",
                table: "ProductEntries");
        }
    }
}
