using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class TenantBecomesOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Activity_Campaign_CampaignId", table: "Activity");
            migrationBuilder.DropForeignKey(name: "FK_ActivitySkill_Activity_ActivityId", table: "ActivitySkill");
            migrationBuilder.DropForeignKey(name: "FK_ActivitySkill_Skill_SkillId", table: "ActivitySkill");
            migrationBuilder.DropForeignKey(name: "FK_AllReadyTask_Tenant_TenantId", table: "AllReadyTask");
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_Tenant_TenantId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_Campaign_Tenant_ManagingTenantId", table: "Campaign");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Campaign_CampaignId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Contact_ContactId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignSponsors_Tenant_TenantId", table: "CampaignSponsors");
            migrationBuilder.DropForeignKey(name: "FK_Skill_Tenant_OwningOrganizationId", table: "Skill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_Skill_SkillId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_AllReadyTask_TaskId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_Skill_SkillId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_ApplicationUser_UserId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "TenantId", table: "CampaignSponsors");
            migrationBuilder.DropColumn(name: "ManagingTenantId", table: "Campaign");
            migrationBuilder.DropColumn(name: "TenantId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "TenantId", table: "AllReadyTask");
            migrationBuilder.DropTable("TenantContact");
            migrationBuilder.DropTable("Tenant");
            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<int>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    WebUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "OrganizationContact",
                columns: table => new
                {
                    OrganizationId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationContact", x => new { x.OrganizationId, x.ContactId, x.ContactType });
                    table.ForeignKey(
                        name: "FK_OrganizationContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationContact_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "CampaignSponsors",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "ManagingOrganizationId",
                table: "Campaign",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AllReadyTask",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Campaign_CampaignId",
                table: "Activity",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySkill_Activity_ActivityId",
                table: "ActivitySkill",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySkill_Skill_SkillId",
                table: "ActivitySkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_AllReadyTask_Organization_OrganizationId",
                table: "AllReadyTask",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Organization_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Organization_ManagingOrganizationId",
                table: "Campaign",
                column: "ManagingOrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContact_Campaign_CampaignId",
                table: "CampaignContact",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContact_Contact_ContactId",
                table: "CampaignContact",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignSponsors_Organization_OrganizationId",
                table: "CampaignSponsors",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Organization_OwningOrganizationId",
                table: "Skill",
                column: "OwningOrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_Skill_SkillId",
                table: "TaskSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_AllReadyTask_TaskId",
                table: "TaskSkill",
                column: "TaskId",
                principalTable: "AllReadyTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_Skill_SkillId",
                table: "UserSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_ApplicationUser_UserId",
                table: "UserSkill",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Activity_Campaign_CampaignId", table: "Activity");
            migrationBuilder.DropForeignKey(name: "FK_ActivitySkill_Activity_ActivityId", table: "ActivitySkill");
            migrationBuilder.DropForeignKey(name: "FK_ActivitySkill_Skill_SkillId", table: "ActivitySkill");
            migrationBuilder.DropForeignKey(name: "FK_AllReadyTask_Organization_OrganizationId", table: "AllReadyTask");
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_Organization_OrganizationId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_Campaign_Organization_ManagingOrganizationId", table: "Campaign");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Campaign_CampaignId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Contact_ContactId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignSponsors_Organization_OrganizationId", table: "CampaignSponsors");
            migrationBuilder.DropForeignKey(name: "FK_Skill_Organization_OwningOrganizationId", table: "Skill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_Skill_SkillId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_AllReadyTask_TaskId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_Skill_SkillId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_ApplicationUser_UserId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "OrganizationId", table: "CampaignSponsors");
            migrationBuilder.DropColumn(name: "ManagingOrganizationId", table: "Campaign");
            migrationBuilder.DropColumn(name: "OrganizationId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "OrganizationId", table: "AllReadyTask");
            migrationBuilder.DropTable("OrganizationContact");
            migrationBuilder.DropTable("Organization");
            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<int>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    WebUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenant_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantContact_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CampaignSponsors",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "ManagingTenantId",
                table: "Campaign",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AllReadyTask",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Campaign_CampaignId",
                table: "Activity",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySkill_Activity_ActivityId",
                table: "ActivitySkill",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySkill_Skill_SkillId",
                table: "ActivitySkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_AllReadyTask_Tenant_TenantId",
                table: "AllReadyTask",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Tenant_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Tenant_ManagingTenantId",
                table: "Campaign",
                column: "ManagingTenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContact_Campaign_CampaignId",
                table: "CampaignContact",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContact_Contact_ContactId",
                table: "CampaignContact",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_CampaignSponsors_Tenant_TenantId",
                table: "CampaignSponsors",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Tenant_OwningOrganizationId",
                table: "Skill",
                column: "OwningOrganizationId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_Skill_SkillId",
                table: "TaskSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_AllReadyTask_TaskId",
                table: "TaskSkill",
                column: "TaskId",
                principalTable: "AllReadyTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_Skill_SkillId",
                table: "UserSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UserSkill_ApplicationUser_UserId",
                table: "UserSkill",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
