using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddBasePriceToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "OrderItems",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            // Backfill existing order items: derive the per-unit price they were actually charged
            // from the stored SubTotal, since that reflects the price at order time.
            migrationBuilder.Sql(@"
                UPDATE OrderItems
                SET BasePrice = SubTotal / Quantity
                WHERE Quantity > 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "OrderItems");
        }
    }
}
