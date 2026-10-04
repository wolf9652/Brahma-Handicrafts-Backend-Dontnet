using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class RequireDesignAndSizeHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Data fix: bring existing rows in line with the new "every Product has >=1 Design,
            // every Design has >=1 Size" invariant before the NOT NULL constraints below are applied. ---

            // 1) Create one "Default" Design for every Product that currently has zero Designs.
            migrationBuilder.Sql(@"
                INSERT INTO Designs (Id, DesignName, DesignDescription, ProductId, CreatedAt, UpdatedAt)
                SELECT NEWID(), 'Default', '', p.Id, SYSUTCDATETIME(), SYSUTCDATETIME()
                FROM Products p
                WHERE NOT EXISTS (SELECT 1 FROM Designs d WHERE d.ProductId = p.Id);
            ");

            // 2) Re-point any Size with a null DesignId (old 'Sizes only' products) to its Product's Design.
            migrationBuilder.Sql(@"
                UPDATE Sizes
                SET DesignId = (SELECT TOP 1 d.Id FROM Designs d WHERE d.ProductId = Sizes.ProductId ORDER BY d.CreatedAt)
                WHERE DesignId IS NULL;
            ");

            // 3) Create one "Default" Size (priced from the Product) for every Product that still has zero Sizes.
            migrationBuilder.Sql(@"
                INSERT INTO Sizes (Id, SizeName, Length, Breadth, Height, Weight, BasePrice, MRP, ProductId, DesignId, CreatedAt, UpdatedAt)
                SELECT NEWID(), 'Default', 0, 0, 0, 0, p.DefaultPrice, p.DefaultMRP, p.Id,
                       (SELECT TOP 1 d.Id FROM Designs d WHERE d.ProductId = p.Id ORDER BY d.CreatedAt),
                       SYSUTCDATETIME(), SYSUTCDATETIME()
                FROM Products p
                WHERE NOT EXISTS (SELECT 1 FROM Sizes s WHERE s.ProductId = p.Id);
            ");

            // 4) Re-point any InventoryStock with a null SizeId (old 'flat product' inventory) to that
            //    Product's (now guaranteed) Default Size and its Design.
            migrationBuilder.Sql(@"
                UPDATE InventoryStocks
                SET SizeId = (SELECT TOP 1 s.Id FROM Sizes s WHERE s.ProductId = InventoryStocks.ProductId ORDER BY s.CreatedAt),
                    DesignId = (SELECT TOP 1 s.DesignId FROM Sizes s WHERE s.ProductId = InventoryStocks.ProductId ORDER BY s.CreatedAt)
                WHERE SizeId IS NULL;
            ");

            // 5) Fill DesignId on any InventoryStock row that already had a SizeId but a null DesignId
            //    (old 'Sizes only' inventory), derived from that Size's now-required Design.
            migrationBuilder.Sql(@"
                UPDATE InventoryStocks
                SET DesignId = (SELECT s.DesignId FROM Sizes s WHERE s.Id = InventoryStocks.SizeId)
                WHERE DesignId IS NULL AND SizeId IS NOT NULL;
            ");

            // 6) Re-point any ProductImage with a null DesignId (old product-level images) to its
            //    Product's (now guaranteed) Design.
            migrationBuilder.Sql(@"
                UPDATE ProductImages
                SET DesignId = (SELECT TOP 1 d.Id FROM Designs d WHERE d.ProductId = ProductImages.ProductId ORDER BY d.CreatedAt)
                WHERE DesignId IS NULL;
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "Sizes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SizeId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "Sizes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "SizeId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DesignId",
                table: "InventoryStocks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
