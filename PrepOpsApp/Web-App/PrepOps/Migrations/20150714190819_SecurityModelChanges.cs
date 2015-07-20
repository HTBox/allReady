using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace PrepOps.Migrations
{
    public partial class SecurityModelChanges : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.AlterColumn(
                name: "CampaignId",
                table: "Activity",
                type: "int",
                nullable: false);
            migration.AlterColumn(
                name: "TenantId",
                table: "Activity",
                type: "int",
                nullable: false);
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.AlterColumn(
                name: "CampaignId",
                table: "Activity",
                type: "int",
                nullable: true);
            migration.AlterColumn(
                name: "TenantId",
                table: "Activity",
                type: "int",
                nullable: true);
        }
    }
}
