using System;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class AddingRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_AllReadyTask_Event_EventId", table: "AllReadyTask");
            migrationBuilder.DropForeignKey(name: "FK_Campaign_Organization_ManagingOrganizationId", table: "Campaign");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Campaign_CampaignId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Contact_ContactId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_Event_Campaign_CampaignId", table: "Event");
            migrationBuilder.DropForeignKey(name: "FK_EventSignup_Event_EventId", table: "EventSignup");
            migrationBuilder.DropForeignKey(name: "FK_EventSkill_Event_EventId", table: "EventSkill");
            migrationBuilder.DropForeignKey(name: "FK_EventSkill_Skill_SkillId", table: "EventSkill");
            migrationBuilder.DropForeignKey(name: "FK_OrganizationContact_Contact_ContactId", table: "OrganizationContact");
            migrationBuilder.DropForeignKey(name: "FK_OrganizationContact_Organization_OrganizationId", table: "OrganizationContact");
            migrationBuilder.DropForeignKey(name: "FK_TaskSignup_AllReadyTask_TaskId", table: "TaskSignup");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_Skill_SkillId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_AllReadyTask_TaskId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_Skill_SkillId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_ApplicationUser_UserId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Lattitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    ProviderData = table.Column<string>(nullable: true),
                    ProviderId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestId);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_AllReadyTask_Event_EventId",
                table: "AllReadyTask",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                name: "FK_Event_Campaign_CampaignId",
                table: "Event",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSignup_Event_EventId",
                table: "EventSignup",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSkill_Event_EventId",
                table: "EventSkill",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSkill_Skill_SkillId",
                table: "EventSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationContact_Contact_ContactId",
                table: "OrganizationContact",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationContact_Organization_OrganizationId",
                table: "OrganizationContact",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSignup_AllReadyTask_TaskId",
                table: "TaskSignup",
                column: "TaskId",
                principalTable: "AllReadyTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
            migrationBuilder.DropForeignKey(name: "FK_AllReadyTask_Event_EventId", table: "AllReadyTask");
            migrationBuilder.DropForeignKey(name: "FK_Campaign_Organization_ManagingOrganizationId", table: "Campaign");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Campaign_CampaignId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_CampaignContact_Contact_ContactId", table: "CampaignContact");
            migrationBuilder.DropForeignKey(name: "FK_Event_Campaign_CampaignId", table: "Event");
            migrationBuilder.DropForeignKey(name: "FK_EventSignup_Event_EventId", table: "EventSignup");
            migrationBuilder.DropForeignKey(name: "FK_EventSkill_Event_EventId", table: "EventSkill");
            migrationBuilder.DropForeignKey(name: "FK_EventSkill_Skill_SkillId", table: "EventSkill");
            migrationBuilder.DropForeignKey(name: "FK_OrganizationContact_Contact_ContactId", table: "OrganizationContact");
            migrationBuilder.DropForeignKey(name: "FK_OrganizationContact_Organization_OrganizationId", table: "OrganizationContact");
            migrationBuilder.DropForeignKey(name: "FK_TaskSignup_AllReadyTask_TaskId", table: "TaskSignup");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_Skill_SkillId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_TaskSkill_AllReadyTask_TaskId", table: "TaskSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_Skill_SkillId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_UserSkill_ApplicationUser_UserId", table: "UserSkill");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropTable("Request");
            migrationBuilder.AddForeignKey(
                name: "FK_AllReadyTask_Event_EventId",
                table: "AllReadyTask",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Organization_ManagingOrganizationId",
                table: "Campaign",
                column: "ManagingOrganizationId",
                principalTable: "Organization",
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
                name: "FK_Event_Campaign_CampaignId",
                table: "Event",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSignup_Event_EventId",
                table: "EventSignup",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSkill_Event_EventId",
                table: "EventSkill",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventSkill_Skill_SkillId",
                table: "EventSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationContact_Contact_ContactId",
                table: "OrganizationContact",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationContact_Organization_OrganizationId",
                table: "OrganizationContact",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TaskSignup_AllReadyTask_TaskId",
                table: "TaskSignup",
                column: "TaskId",
                principalTable: "AllReadyTask",
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
