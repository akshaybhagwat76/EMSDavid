using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Made_QuotationID_Nullable_QuotationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails");

            migrationBuilder.AlterColumn<int>(
                name: "QuotationId",
                table: "QuotationDetails",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails");

            migrationBuilder.AlterColumn<int>(
                name: "QuotationId",
                table: "QuotationDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Quotations_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
