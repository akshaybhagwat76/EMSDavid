using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Regenerated_AssetPart4189 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AssetParts",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_ParentId",
                table: "AssetParts",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_AssetParts_ParentId",
                table: "AssetParts",
                column: "ParentId",
                principalTable: "AssetParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_AssetParts_ParentId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_ParentId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "AssetParts");
        }
    }
}
