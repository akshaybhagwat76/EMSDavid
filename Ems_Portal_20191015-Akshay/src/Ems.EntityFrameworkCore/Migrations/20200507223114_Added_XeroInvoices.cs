using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_XeroInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "XeroInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    XeroInvoiceCreated = table.Column<bool>(nullable: false),
                    ApiResponse = table.Column<string>(nullable: true),
                    Failed = table.Column<bool>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    XeroReference = table.Column<string>(nullable: true),
                    CustomerInvoiceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XeroInvoices_CustomerInvoices_CustomerInvoiceId",
                        column: x => x.CustomerInvoiceId,
                        principalTable: "CustomerInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XeroInvoices_CustomerInvoiceId",
                table: "XeroInvoices",
                column: "CustomerInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_XeroInvoices_TenantId",
                table: "XeroInvoices",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XeroInvoices");
        }
    }
}
