using System;
using System.Collections.Generic;
using AllReady.Models;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class ImpactTypeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_CampaignImpact_CampaignImpactType_CampaignImpactTypeId", table: "CampaignImpact");
            migrationBuilder.DropColumn(name: "CampaignImpactTypeId", table: "CampaignImpact");
            migrationBuilder.DropTable("CampaignImpactType");
            migrationBuilder.AddColumn<int>(
                name: "ImpactType",
                table: "CampaignImpact",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImpactType", table: "CampaignImpact");
            migrationBuilder.CreateTable(
                name: "CampaignImpactType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpactType", x => x.Id);
                });
            migrationBuilder.AddColumn<int>(
                name: "CampaignImpactTypeId",
                table: "CampaignImpact",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignImpact_CampaignImpactType_CampaignImpactTypeId",
                table: "CampaignImpact",
                column: "CampaignImpactTypeId",
                principalTable: "CampaignImpactType",
                principalColumn: "Id");
        }
    }
}
