using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKhderApi.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityColumnInProductCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductCart",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductCart");
        }
    }
}
