using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WoDetails_In_WoUpdates_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "WorkOrderUpdates",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "WorkOrderUpdates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderUpdates_ItemTypeId",
                table: "WorkOrderUpdates",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderUpdates_UomId",
                table: "WorkOrderUpdates",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderUpdates_ItemTypes_ItemTypeId",
                table: "WorkOrderUpdates",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderUpdates_Uoms_UomId",
                table: "WorkOrderUpdates",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderUpdates_ItemTypes_ItemTypeId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderUpdates_Uoms_UomId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderUpdates_ItemTypeId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderUpdates_UomId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "WorkOrderUpdates");
        }
    }
}
