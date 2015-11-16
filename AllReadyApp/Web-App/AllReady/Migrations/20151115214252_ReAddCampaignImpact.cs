using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class ReAddCampaignImpact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignImpact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentImpactLevel = table.Column<int>(nullable: false),
                    Display = table.Column<bool>(nullable: false),
                    ImpactType = table.Column<int>(nullable: false),
                    NumericImpactGoal = table.Column<int>(nullable: false),
                    TextualImpactGoal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpact", x => x.Id);
                });
            migrationBuilder.AddColumn<int>(
                name: "CampaignImpactId",
                table: "Campaign",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_CampaignImpact_CampaignImpactId",
                table: "Campaign",
                column: "CampaignImpactId",
                principalTable: "CampaignImpact",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Campaign_CampaignImpact_CampaignImpactId", table: "Campaign");
            migrationBuilder.DropColumn(name: "CampaignImpactId", table: "Campaign");
            migrationBuilder.DropTable("CampaignImpact");
        }
    }
}
