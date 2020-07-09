using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Removed_EstimateID_InQuotationDetails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Estimates_EstimateId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_EstimateId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "EstimateId",
                table: "QuotationDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimateId",
                table: "QuotationDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_EstimateId",
                table: "QuotationDetails",
                column: "EstimateId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Estimates_EstimateId",
                table: "QuotationDetails",
                column: "EstimateId",
                principalTable: "Estimates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
