using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Moved_WO_Asset_From_Quote_To_Details : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_AssetClasses_AssetClassId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Assets_AssetId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_SupportItems_SupportItemId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_SupportTypes_SupportTypeId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_WorkOrders_WorkOrderId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_AssetClassId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_AssetId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_SupportItemId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_SupportTypeId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_WorkOrderId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "AssetClassId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "SupportItemId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "SupportTypeId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "QuotationDetails");

            migrationBuilder.AddColumn<int>(
                name: "AssetClassId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupportItemId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupportTypeId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuotationId",
                table: "QuotationDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_AssetClassId",
                table: "Quotations",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_AssetId",
                table: "Quotations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_SupportItemId",
                table: "Quotations",
                column: "SupportItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_SupportTypeId",
                table: "Quotations",
                column: "SupportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_AssetClasses_AssetClassId",
                table: "Quotations",
                column: "AssetClassId",
                principalTable: "AssetClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Assets_AssetId",
                table: "Quotations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_SupportItems_SupportItemId",
                table: "Quotations",
                column: "SupportItemId",
                principalTable: "SupportItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_SupportTypes_SupportTypeId",
                table: "Quotations",
                column: "SupportTypeId",
                principalTable: "SupportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_AssetClasses_AssetClassId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Assets_AssetId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_SupportItems_SupportItemId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_SupportTypes_SupportTypeId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_AssetClassId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_AssetId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_SupportItemId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_SupportTypeId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "AssetClassId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "SupportItemId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "SupportTypeId",
                table: "Quotations");

            migrationBuilder.AlterColumn<int>(
                name: "QuotationId",
                table: "QuotationDetails",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AssetClassId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupportItemId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupportTypeId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_AssetClassId",
                table: "QuotationDetails",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_AssetId",
                table: "QuotationDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_SupportItemId",
                table: "QuotationDetails",
                column: "SupportItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_SupportTypeId",
                table: "QuotationDetails",
                column: "SupportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_WorkOrderId",
                table: "QuotationDetails",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_AssetClasses_AssetClassId",
                table: "QuotationDetails",
                column: "AssetClassId",
                principalTable: "AssetClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Assets_AssetId",
                table: "QuotationDetails",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_SupportItems_SupportItemId",
                table: "QuotationDetails",
                column: "SupportItemId",
                principalTable: "SupportItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_SupportTypes_SupportTypeId",
                table: "QuotationDetails",
                column: "SupportTypeId",
                principalTable: "SupportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_WorkOrders_WorkOrderId",
                table: "QuotationDetails",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
