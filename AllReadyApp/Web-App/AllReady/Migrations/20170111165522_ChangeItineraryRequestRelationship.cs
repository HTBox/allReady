using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class ChangeItineraryRequestRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItineraryRequest_RequestId",
                table: "ItineraryRequest");

            migrationBuilder.AddColumn<int>(
                name: "ItineraryId",
                table: "Request",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_RequestId",
                table: "ItineraryRequest",
                column: "RequestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItineraryRequest_RequestId",
                table: "ItineraryRequest");

            migrationBuilder.DropColumn(
                name: "ItineraryId",
                table: "Request");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_RequestId",
                table: "ItineraryRequest",
                column: "RequestId");
        }
    }
}
