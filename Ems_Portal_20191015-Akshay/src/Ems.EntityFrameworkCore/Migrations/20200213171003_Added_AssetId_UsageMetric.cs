using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_AssetId_UsageMetric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "UsageMetrics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UsageMetrics_AssetId",
                table: "UsageMetrics",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageMetrics_Assets_AssetId",
                table: "UsageMetrics",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsageMetrics_Assets_AssetId",
                table: "UsageMetrics");

            migrationBuilder.DropIndex(
                name: "IX_UsageMetrics_AssetId",
                table: "UsageMetrics");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "UsageMetrics");
        }
    }
}
