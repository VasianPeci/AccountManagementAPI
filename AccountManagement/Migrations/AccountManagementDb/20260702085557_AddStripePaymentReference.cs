using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagement.Migrations.AccountManagementDb
{
    /// <inheritdoc />
    public partial class AddStripePaymentReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePaymentId",
                table: "BankTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_StripePaymentId",
                table: "BankTransactions",
                column: "StripePaymentId",
                unique: true,
                filter: "[StripePaymentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_StripePaymentId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "StripePaymentId",
                table: "BankTransactions");
        }
    }
}
