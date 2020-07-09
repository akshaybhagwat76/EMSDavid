using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WorkOrderID_In_Estimates_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "Estimates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderActions_TenantId",
                table: "WorkOrderActions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_WorkOrderId",
                table: "Estimates",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_WorkOrders_WorkOrderId",
                table: "Estimates",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_WorkOrders_WorkOrderId",
                table: "Estimates");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderActions_TenantId",
                table: "WorkOrderActions");

            migrationBuilder.DropIndex(
                name: "IX_Estimates_WorkOrderId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "Estimates");
        }
    }
}
