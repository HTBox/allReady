using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class ImpactTypeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CampaignImpactType", table: "CampaignImpact");
            migrationBuilder.AddColumn<int>(
                name: "ImpactType",
                table: "CampaignImpact",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImpactType", table: "CampaignImpact");
            migrationBuilder.AddColumn<int>(
                name: "CampaignImpactType",
                table: "CampaignImpact",
                nullable: false,
                defaultValue: 0);
        }
    }
}
