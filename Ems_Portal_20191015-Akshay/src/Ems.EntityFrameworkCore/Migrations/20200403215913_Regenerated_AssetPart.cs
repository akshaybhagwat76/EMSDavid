using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Regenerated_AssetPart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetPartStatusId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttachmentId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Installed",
                table: "AssetParts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageMetricId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_AssetId",
                table: "AssetParts",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_AssetPartStatusId",
                table: "AssetParts",
                column: "AssetPartStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_AttachmentId",
                table: "AssetParts",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_ItemTypeId",
                table: "AssetParts",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_UsageMetricId",
                table: "AssetParts",
                column: "UsageMetricId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_Assets_AssetId",
                table: "AssetParts",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_AssetPartStatuses_AssetPartStatusId",
                table: "AssetParts",
                column: "AssetPartStatusId",
                principalTable: "AssetPartStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_Attachments_AttachmentId",
                table: "AssetParts",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_ItemTypes_ItemTypeId",
                table: "AssetParts",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_UsageMetrics_UsageMetricId",
                table: "AssetParts",
                column: "UsageMetricId",
                principalTable: "UsageMetrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_Assets_AssetId",
                table: "AssetParts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_AssetPartStatuses_AssetPartStatusId",
                table: "AssetParts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_Attachments_AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_ItemTypes_ItemTypeId",
                table: "AssetParts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_UsageMetrics_UsageMetricId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_AssetId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_AssetPartStatusId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_ItemTypeId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_UsageMetricId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "AssetPartStatusId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "Installed",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "UsageMetricId",
                table: "AssetParts");
        }
    }
}
