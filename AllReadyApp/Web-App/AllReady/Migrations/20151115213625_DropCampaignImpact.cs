using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class DropCampaignImpact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("CampaignImpact");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignImpact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CurrentImpactLevel = table.Column<int>(nullable: false),
                    Display = table.Column<bool>(nullable: false),
                    ImpactType = table.Column<int>(nullable: false),
                    NumericImpactGoal = table.Column<int>(nullable: false),
                    TextualImpactGoal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignImpact_Campaign_Id",
                        column: x => x.Id,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                });
        }
    }
}
