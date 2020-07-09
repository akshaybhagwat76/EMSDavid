using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Updated_AssetPart_CodeNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetParts",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetParts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
