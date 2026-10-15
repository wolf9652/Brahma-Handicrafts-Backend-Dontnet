using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSizeIdFromWishlistItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Sizes_SizeId",
                table: "WishlistItems");

            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_SizeId",
                table: "WishlistItems");

            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_UserId_ProductId_DesignId_SizeId",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "WishlistItems");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_UserId_ProductId_DesignId",
                table: "WishlistItems",
                columns: new[] { "UserId", "ProductId", "DesignId" },
                unique: true,
                filter: "[DesignId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_UserId_ProductId_DesignId",
                table: "WishlistItems");

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "WishlistItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_SizeId",
                table: "WishlistItems",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_UserId_ProductId_DesignId_SizeId",
                table: "WishlistItems",
                columns: new[] { "UserId", "ProductId", "DesignId", "SizeId" },
                unique: true,
                filter: "[DesignId] IS NOT NULL AND [SizeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Sizes_SizeId",
                table: "WishlistItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
