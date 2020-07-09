using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Moved_LeaseItemId_To_InvoiceDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_LeaseItemId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "LeaseItemId",
                table: "CustomerInvoices");

            migrationBuilder.AddColumn<int>(
                name: "LeaseItemId",
                table: "CustomerInvoiceDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceDetails_LeaseItemId",
                table: "CustomerInvoiceDetails",
                column: "LeaseItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceDetails_LeaseItems_LeaseItemId",
                table: "CustomerInvoiceDetails",
                column: "LeaseItemId",
                principalTable: "LeaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceDetails_LeaseItems_LeaseItemId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoiceDetails_LeaseItemId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "LeaseItemId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.AddColumn<int>(
                name: "LeaseItemId",
                table: "CustomerInvoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_LeaseItemId",
                table: "CustomerInvoices",
                column: "LeaseItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices",
                column: "LeaseItemId",
                principalTable: "LeaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
