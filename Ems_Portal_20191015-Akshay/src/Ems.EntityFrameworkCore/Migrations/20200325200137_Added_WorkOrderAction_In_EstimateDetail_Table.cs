using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WorkOrderAction_In_EstimateDetail_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EstimateDetails",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderActionId",
                table: "EstimateDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDetails_WorkOrderActionId",
                table: "EstimateDetails",
                column: "WorkOrderActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EstimateDetails_WorkOrderActions_WorkOrderActionId",
                table: "EstimateDetails",
                column: "WorkOrderActionId",
                principalTable: "WorkOrderActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstimateDetails_WorkOrderActions_WorkOrderActionId",
                table: "EstimateDetails");

            migrationBuilder.DropIndex(
                name: "IX_EstimateDetails_WorkOrderActionId",
                table: "EstimateDetails");

            migrationBuilder.DropColumn(
                name: "WorkOrderActionId",
                table: "EstimateDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EstimateDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
