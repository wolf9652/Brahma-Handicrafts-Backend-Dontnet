using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BRAHMA_HANDICRAFT_BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerNameAndPhoneToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Addresses",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "Addresses",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            // Backfill existing rows from their owning User's own name/phone number.
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.CustomerName = u.FirstName + ' ' + u.LastName,
                    a.CustomerPhoneNumber = u.PhoneNumber
                FROM Addresses a
                INNER JOIN Users u ON u.Id = a.UserId;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "Addresses");
        }
    }
}
