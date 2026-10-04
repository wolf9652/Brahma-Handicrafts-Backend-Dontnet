using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDescriptionAndDefaultPricesFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultMRP",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DefaultPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMRP",
                table: "Products",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPrice",
                table: "Products",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "");
        }
    }
}
