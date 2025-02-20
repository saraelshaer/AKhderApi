using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCartCarbonFootprintApi.Migrations
{
    /// <inheritdoc />
    public partial class fixCartRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Carts_CartId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CartId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CartId1",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartId1",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CartId1",
                table: "AspNetUsers",
                column: "CartId1",
                unique: true,
                filter: "[CartId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Carts_CartId1",
                table: "AspNetUsers",
                column: "CartId1",
                principalTable: "Carts",
                principalColumn: "Id");
        }
    }
}
