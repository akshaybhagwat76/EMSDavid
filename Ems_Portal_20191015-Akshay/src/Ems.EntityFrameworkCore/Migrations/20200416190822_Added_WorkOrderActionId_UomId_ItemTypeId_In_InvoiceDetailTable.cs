using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WorkOrderActionId_UomId_ItemTypeId_In_InvoiceDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices");

            migrationBuilder.AlterColumn<int>(
                name: "LeaseItemId",
                table: "CustomerInvoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "CustomerInvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "CustomerInvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderActionId",
                table: "CustomerInvoiceDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceDetails_ItemTypeId",
                table: "CustomerInvoiceDetails",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceDetails_UomId",
                table: "CustomerInvoiceDetails",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoiceDetails_WorkOrderActionId",
                table: "CustomerInvoiceDetails",
                column: "WorkOrderActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceDetails_ItemTypes_ItemTypeId",
                table: "CustomerInvoiceDetails",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceDetails_Uoms_UomId",
                table: "CustomerInvoiceDetails",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoiceDetails_WorkOrderActions_WorkOrderActionId",
                table: "CustomerInvoiceDetails",
                column: "WorkOrderActionId",
                principalTable: "WorkOrderActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices",
                column: "LeaseItemId",
                principalTable: "LeaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceDetails_ItemTypes_ItemTypeId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceDetails_Uoms_UomId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoiceDetails_WorkOrderActions_WorkOrderActionId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoiceDetails_ItemTypeId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoiceDetails_UomId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoiceDetails_WorkOrderActionId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "WorkOrderActionId",
                table: "CustomerInvoiceDetails");

            migrationBuilder.AlterColumn<int>(
                name: "LeaseItemId",
                table: "CustomerInvoices",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_LeaseItems_LeaseItemId",
                table: "CustomerInvoices",
                column: "LeaseItemId",
                principalTable: "LeaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
