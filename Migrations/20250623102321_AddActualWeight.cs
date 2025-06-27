using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKhderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddActualWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualWeight",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualWeight",
                table: "Carts");
        }
    }
}
