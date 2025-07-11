using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CeramicaCanelas.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesFixProducts3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValueUnit",
                table: "Products",
                newName: "ValueTotal");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ValueTotal",
                table: "Products",
                newName: "ValueUnit");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
