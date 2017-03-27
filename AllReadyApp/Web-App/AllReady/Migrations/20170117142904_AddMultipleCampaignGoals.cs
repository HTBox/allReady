using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddMultipleCampaignGoals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "CampaignImpact",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
@"UPDATE i SET i.[CampaignId] = c.[CampaignImpactId]
  FROM [CampaignImpact] i
       INNER JOIN [Campaign] c ON i.Id = c.[CampaignImpactId]");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_CampaignImpact_CampaignImpactId",
                table: "Campaign");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_CampaignImpactId",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "CampaignImpactId",
                table: "Campaign");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignImpact_CampaignId",
                table: "CampaignImpact",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignImpact_Campaign_CampaignId",
                table: "CampaignImpact",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignImpact_Campaign_CampaignId",
                table: "CampaignImpact");

            migrationBuilder.AddColumn<int>(
                name: "CampaignImpactId",
                table: "Campaign",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE c SET c.[CampaignImpactId] = i.[Id]
  FROM [Campaign] c
       INNER JOIN [CampaignImpact] i ON i.CampaignId = c.[Id]");

            migrationBuilder.DropIndex(
                name: "IX_CampaignImpact_CampaignId",
                table: "CampaignImpact");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "CampaignImpact");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_CampaignImpactId",
                table: "Campaign",
                column: "CampaignImpactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_CampaignImpact_CampaignImpactId",
                table: "Campaign",
                column: "CampaignImpactId",
                principalTable: "CampaignImpact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
