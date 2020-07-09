using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_EstimateDetails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstimateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Cost = table.Column<decimal>(nullable: true),
                    Tax = table.Column<decimal>(nullable: false),
                    Charge = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    MarkUp = table.Column<decimal>(nullable: false),
                    IsChargeable = table.Column<bool>(nullable: false),
                    IsAdHoc = table.Column<bool>(nullable: false),
                    IsStandbyReplacementUnit = table.Column<bool>(nullable: false),
                    IsOptionalItem = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Loc8GUID = table.Column<string>(nullable: true),
                    ItemTypeId = table.Column<int>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    EstimateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateDetails_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstimateDetails_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimateDetails_Uoms_UomId",
                        column: x => x.UomId,
                        principalTable: "Uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDetails_EstimateId",
                table: "EstimateDetails",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDetails_ItemTypeId",
                table: "EstimateDetails",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDetails_TenantId",
                table: "EstimateDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateDetails_UomId",
                table: "EstimateDetails",
                column: "UomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstimateDetails");
        }
    }
}
