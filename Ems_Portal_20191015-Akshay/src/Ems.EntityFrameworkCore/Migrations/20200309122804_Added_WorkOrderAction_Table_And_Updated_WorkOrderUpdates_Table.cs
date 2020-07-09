using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_WorkOrderAction_Table_And_Updated_WorkOrderUpdates_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderUpdates_Uoms_UomId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderUpdates_UomId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "Complete",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "Update",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "WorkOrderUpdates");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "WorkOrderUpdates",
                newName: "Number");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "WorkOrderUpdates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderActionId",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorkOrderActions",
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
                    Action = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderActions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderUpdates_WorkOrderActionId",
                table: "WorkOrderUpdates",
                column: "WorkOrderActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderUpdates_WorkOrderActions_WorkOrderActionId",
                table: "WorkOrderUpdates",
                column: "WorkOrderActionId",
                principalTable: "WorkOrderActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderUpdates_WorkOrderActions_WorkOrderActionId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropTable(
                name: "WorkOrderActions");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderUpdates_WorkOrderActionId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "WorkOrderActionId",
                table: "WorkOrderUpdates");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "WorkOrderUpdates",
                newName: "Quantity");

            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "WorkOrderUpdates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderUpdates_UomId",
                table: "WorkOrderUpdates",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderUpdates_Uoms_UomId",
                table: "WorkOrderUpdates",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
