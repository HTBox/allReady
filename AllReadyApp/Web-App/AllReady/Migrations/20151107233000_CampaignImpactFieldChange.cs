using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class CampaignImpactFieldChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImpactType", table: "CampaignImpactType");
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CampaignImpactType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Name", table: "CampaignImpactType");
            migrationBuilder.AddColumn<string>(
                name: "ImpactType",
                table: "CampaignImpactType",
                nullable: true);
        }
    }
}
