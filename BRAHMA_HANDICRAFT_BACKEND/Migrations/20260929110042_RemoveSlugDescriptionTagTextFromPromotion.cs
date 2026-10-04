using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSlugDescriptionTagTextFromPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_Slug",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "TagText",
                table: "Promotions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Promotions",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Promotions",
                type: "varchar(150)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagText",
                table: "Promotions",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Slug",
                table: "Promotions",
                column: "Slug",
                unique: true);
        }
    }
}
