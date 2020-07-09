using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WorkOrderId_EstimateId_In_InvoiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimateId",
                table: "CustomerInvoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "CustomerInvoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_EstimateId",
                table: "CustomerInvoices",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_WorkOrderId",
                table: "CustomerInvoices",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_Estimates_EstimateId",
                table: "CustomerInvoices",
                column: "EstimateId",
                principalTable: "Estimates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_WorkOrders_WorkOrderId",
                table: "CustomerInvoices",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_Estimates_EstimateId",
                table: "CustomerInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_WorkOrders_WorkOrderId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_EstimateId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_WorkOrderId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "EstimateId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "CustomerInvoices");
        }
    }
}
