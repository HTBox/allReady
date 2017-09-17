using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AllReady.Migrations
{
    public partial class _200Updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request");

            migrationBuilder.AddColumn<int>(
                name: "ItineraryId1",
                table: "ItineraryRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Campaign",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request",
                columns: new[] { "ItineraryId1", "ItineraryRequestId" });

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_ItineraryId1",
                table: "ItineraryRequest",
                column: "ItineraryId1");

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
                name: "FK_ItineraryRequest_Itinerary_ItineraryId1",
                table: "ItineraryRequest",
                column: "ItineraryId1",
                principalTable: "Itinerary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryRequest_Request_RequestId",
                table: "ItineraryRequest",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_Organization_OrganizationId",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryRequest_Itinerary_ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryRequest_Request_RequestId",
                table: "ItineraryRequest");

            migrationBuilder.DropIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryRequest_ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_OrganizationId",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ItineraryId1",
                table: "ItineraryRequest");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Campaign");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ItineraryId1_ItineraryRequestId",
                table: "Request",
                columns: new[] { "ItineraryId1", "ItineraryRequestId" },
                unique: true,
                filter: "[ItineraryId1] IS NOT NULL AND [ItineraryRequestId] IS NOT NULL");
        }
    }
}
