using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_CustomerID_In_Estimates_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Estimates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Estimates_CustomerId",
                table: "Estimates",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_Customers_CustomerId",
                table: "Estimates",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_Customers_CustomerId",
                table: "Estimates");

            migrationBuilder.DropIndex(
                name: "IX_Estimates_CustomerId",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Estimates");
        }
    }
}
