using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedPriceToDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedPrice",
                table: "Designs",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            // Backfill: seed each existing Design's EstimatedPrice from its Product's DefaultPrice.
            migrationBuilder.Sql(@"
                UPDATE d
                SET d.EstimatedPrice = p.DefaultPrice
                FROM Designs d
                INNER JOIN Products p ON p.Id = d.ProductId;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedPrice",
                table: "Designs");
        }
    }
}
