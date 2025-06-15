using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKhderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsForFootprint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Agriculture",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FoodProcessing",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Iluc",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Packaging",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Retail",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Transport",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agriculture",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FoodProcessing",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Iluc",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Packaging",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Retail",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Transport",
                table: "Products");
        }
    }
}
