using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class AddTenantContactCampaignContactContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "CampaignContact",
                columns: table => new
                {
                    CampaignId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignContact", x => new { x.CampaignId, x.ContactId, x.ContactType });
                    table.ForeignKey(
                        name: "FK_CampaignContact_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CampaignContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id");
                });
            migrationBuilder.CreateTable(
                name: "TenantContact",
                columns: table => new
                {
                    TenantId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantContact", x => new { x.TenantId, x.ContactId, x.ContactType });
                    table.ForeignKey(
                        name: "FK_TenantContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenantContact_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id");
                });
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tenant",
                nullable: false);
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Tenant",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Campaign",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AllReadyTask",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Activity",
                nullable: false);
            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Location_LocationId",
                table: "Campaign",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_Tenant_Location_LocationId",
                table: "Tenant",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Campaign_Location_LocationId", table: "Campaign");
            migrationBuilder.DropForeignKey(name: "FK_Tenant_Location_LocationId", table: "Tenant");
            migrationBuilder.DropColumn(name: "LocationId", table: "Tenant");
            migrationBuilder.DropColumn(name: "LocationId", table: "Campaign");
            migrationBuilder.DropTable("CampaignContact");
            migrationBuilder.DropTable("TenantContact");
            migrationBuilder.DropTable("Contact");
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tenant",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AllReadyTask",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Activity",
                nullable: true);
        }
    }
}
