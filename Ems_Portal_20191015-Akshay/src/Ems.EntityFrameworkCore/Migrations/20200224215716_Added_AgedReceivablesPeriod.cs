using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_AgedReceivablesPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgedReceivablesPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Period = table.Column<DateTime>(nullable: false),
                    Current = table.Column<decimal>(nullable: false),
                    Over30 = table.Column<decimal>(nullable: false),
                    Over60 = table.Column<decimal>(nullable: false),
                    Over90 = table.Column<decimal>(nullable: false),
                    Over120 = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgedReceivablesPeriods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgedReceivablesPeriods_TenantId",
                table: "AgedReceivablesPeriods",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgedReceivablesPeriods");
        }
    }
}
