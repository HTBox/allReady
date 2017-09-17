using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AllReady.Migrations
{
    public partial class Post20Fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_Organization_OrganizationId",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityRole_ApplicationUser_ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryRequest_Itinerary_ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryRequest_ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropIndex(
                name: "IX_IdentityUserClaim<string>_ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropIndex(
                name: "IX_IdentityRole_ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_OrganizationId",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "IdentityUserClaim<string>");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "IdentityRole");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Campaign");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItineraryId1",
                table: "ItineraryRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "IdentityUserClaim<string>",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "IdentityRole",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Campaign",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_ItineraryId1",
                table: "ItineraryRequest",
                column: "ItineraryId1");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaim<string>_ApplicationUserId",
                table: "IdentityUserClaim<string>",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRole_ApplicationUserId",
                table: "IdentityRole",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_OrganizationId",
                table: "Campaign",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Organization_OrganizationId",
                table: "Campaign",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_ItineraryRequest_Itinerary_ItineraryId1",
                table: "ItineraryRequest",
                column: "ItineraryId1",
                principalTable: "Itinerary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
