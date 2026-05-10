using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ParcelAllocationNavigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ParcelAllocations_BuyTransactionID",
                table: "ParcelAllocations",
                column: "BuyTransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelAllocations_SellTransactionID",
                table: "ParcelAllocations",
                column: "SellTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ParcelAllocations_Transactions_BuyTransactionID",
                table: "ParcelAllocations",
                column: "BuyTransactionID",
                principalTable: "Transactions",
                principalColumn: "TransactionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParcelAllocations_Transactions_SellTransactionID",
                table: "ParcelAllocations",
                column: "SellTransactionID",
                principalTable: "Transactions",
                principalColumn: "TransactionID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParcelAllocations_Transactions_BuyTransactionID",
                table: "ParcelAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParcelAllocations_Transactions_SellTransactionID",
                table: "ParcelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ParcelAllocations_BuyTransactionID",
                table: "ParcelAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ParcelAllocations_SellTransactionID",
                table: "ParcelAllocations");
        }
    }
}
