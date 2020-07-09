using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Moved_Uom_From_UsageMericRecord_To_UsageMetric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageMetricRecords_Uoms_UomId",
                table: "UsageMetricRecords");

            migrationBuilder.DropIndex(
                name: "IX_UsageMetricRecords_UomId",
                table: "UsageMetricRecords");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "UsageMetricRecords");

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "UsageMetrics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsageMetrics_UomId",
                table: "UsageMetrics",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageMetrics_Uoms_UomId",
                table: "UsageMetrics",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageMetrics_Uoms_UomId",
                table: "UsageMetrics");

            migrationBuilder.DropIndex(
                name: "IX_UsageMetrics_UomId",
                table: "UsageMetrics");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "UsageMetrics");

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "UsageMetricRecords",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsageMetricRecords_UomId",
                table: "UsageMetricRecords",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageMetricRecords_Uoms_UomId",
                table: "UsageMetricRecords",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
