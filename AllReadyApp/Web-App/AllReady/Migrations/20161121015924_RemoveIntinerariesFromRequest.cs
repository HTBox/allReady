using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class RemoveIntinerariesFromRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary");

            migrationBuilder.DropIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary");

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary",
                column: "StartLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary",
                column: "EndLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary");

            migrationBuilder.DropIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary");

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary",
                column: "StartLocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary",
                column: "EndLocationId",
                unique: true);
        }
    }
}
