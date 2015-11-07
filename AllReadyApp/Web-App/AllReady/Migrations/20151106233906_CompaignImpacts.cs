using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class CompaignImpacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignImpactType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImpactType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpactType", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "CampaignImpact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CampaignImpactTypeId = table.Column<int>(nullable: true),
                    CurrentImpactLevel = table.Column<int>(nullable: false),
                    Display = table.Column<bool>(nullable: false),
                    NumericImpactGoal = table.Column<int>(nullable: false),
                    TextualImpactGoal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignImpact_CampaignImpactType_CampaignImpactTypeId",
                        column: x => x.CampaignImpactTypeId,
                        principalTable: "CampaignImpactType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CampaignImpact_Campaign_Id",
                        column: x => x.Id,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                });
            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Campaign",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "FullDescription", table: "Campaign");
            migrationBuilder.DropTable("CampaignImpact");
            migrationBuilder.DropTable("CampaignImpactType");
        }
    }
}
