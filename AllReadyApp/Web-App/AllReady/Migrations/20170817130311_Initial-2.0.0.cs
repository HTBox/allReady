using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AllReady.Migrations
{
    public partial class Initial200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignContact_Contact_ContactId1",
                table: "CampaignContact");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryRequest_Request_RequestId",
                table: "ItineraryRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationContact_Contact_ContactId1",
                table: "OrganizationContact");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerTaskSkill_VolunteerTaskId",
                table: "VolunteerTaskSkill");

            migrationBuilder.DropIndex(
                name: "IX_UserSkill_UserId",
                table: "UserSkill");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationContact_ContactId1",
                table: "OrganizationContact");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationContact_OrganizationId",
                table: "OrganizationContact");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryRequest_ItineraryId",
                table: "ItineraryRequest");

            migrationBuilder.DropIndex(
                name: "IX_EventSkill_EventId",
                table: "EventSkill");

            migrationBuilder.DropIndex(
                name: "IX_EventManager_UserId",
                table: "EventManager");

            migrationBuilder.DropIndex(
                name: "IX_CampaignManager_UserId",
                table: "CampaignManager");

            migrationBuilder.DropIndex(
                name: "IX_CampaignContact_CampaignId",
                table: "CampaignContact");

            migrationBuilder.DropIndex(
                name: "IX_CampaignContact_ContactId1",
                table: "CampaignContact");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_IdentityUserRole<string>_UserId",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "IdentityRole");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "OrganizationContact");

            migrationBuilder.DropColumn(
                name: "ContactId1",
                table: "CampaignContact");

            migrationBuilder.AddColumn<int>(
                name: "ItineraryId1",
                table: "Request",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItineraryRequestId",
                table: "Request",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "IdentityUserClaim<string>",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "IdentityRole",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request",
                columns: new[] { "ItineraryId1", "ItineraryRequestId" },
                unique: true,
                filter: "[ItineraryId1] IS NOT NULL AND [ItineraryRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaim<string>_ApplicationUserId",
                table: "IdentityUserClaim<string>",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRole_ApplicationUserId",
                table: "IdentityRole",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRole_ApplicationUser_ApplicationUserId",
                table: "IdentityRole",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_ApplicationUserId",
                table: "IdentityUserClaim<string>",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserToken<string>_ApplicationUser_UserId",
                table: "IdentityUserToken<string>",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ItineraryRequest_ItineraryId1_ItineraryRequestId",
                table: "Request",
                columns: new[] { "ItineraryId1", "ItineraryRequestId" },
                principalTable: "ItineraryRequest",
                principalColumns: new[] { "ItineraryId", "RequestId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityRole_ApplicationUser_ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserToken<string>_ApplicationUser_UserId",
                table: "IdentityUserToken<string>");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ItineraryRequest_ItineraryId1_ItineraryRequestId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_IdentityUserClaim<string>_ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropIndex(
                name: "IX_IdentityRole_ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "IdentityRole");

            migrationBuilder.DropColumn(
                name: "ItineraryId1",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ItineraryRequestId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.AddColumn<int>(
                name: "ContactId1",
                table: "OrganizationContact",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactId1",
                table: "CampaignContact",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSkill_VolunteerTaskId",
                table: "VolunteerTaskSkill",
                column: "VolunteerTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkill_UserId",
                table: "UserSkill",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContact_ContactId1",
                table: "OrganizationContact",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContact_OrganizationId",
                table: "OrganizationContact",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_ItineraryId",
                table: "ItineraryRequest",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSkill_EventId",
                table: "EventSkill",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManager_UserId",
                table: "EventManager",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignManager_UserId",
                table: "CampaignManager",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContact_CampaignId",
                table: "CampaignContact",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContact_ContactId1",
                table: "CampaignContact",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole<string>_UserId",
                table: "IdentityUserRole<string>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityRole",
                column: "NormalizedName");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignContact_Contact_ContactId1",
                table: "CampaignContact",
                column: "ContactId1",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryRequest_Request_RequestId",
                table: "ItineraryRequest",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationContact_Contact_ContactId1",
                table: "OrganizationContact",
                column: "ContactId1",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
