using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddResourceToCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "Resource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_CampaignId",
                table: "Resource",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Campaign_CampaignId",
                table: "Resource",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Campaign_CampaignId",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_CampaignId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "Resource");
        }
    }
}
