using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class ItineraryAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EndLatitude",
                table: "Itinerary",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EndLocationId",
                table: "Itinerary",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EndLongitude",
                table: "Itinerary",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLatitude",
                table: "Itinerary",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "StartLocationId",
                table: "Itinerary",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StartLongitude",
                table: "Itinerary",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseStartAddressAsEndAddress",
                table: "Itinerary",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary",
                column: "EndLocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary",
                column: "StartLocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Itinerary_Location_EndLocationId",
                table: "Itinerary",
                column: "EndLocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Itinerary_Location_StartLocationId",
                table: "Itinerary",
                column: "StartLocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itinerary_Location_EndLocationId",
                table: "Itinerary");

            migrationBuilder.DropForeignKey(
                name: "FK_Itinerary_Location_StartLocationId",
                table: "Itinerary");

            migrationBuilder.DropIndex(
                name: "IX_Itinerary_EndLocationId",
                table: "Itinerary");

            migrationBuilder.DropIndex(
                name: "IX_Itinerary_StartLocationId",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "EndLatitude",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "EndLocationId",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "EndLongitude",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "StartLatitude",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "StartLocationId",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "StartLongitude",
                table: "Itinerary");

            migrationBuilder.DropColumn(
                name: "UseStartAddressAsEndAddress",
                table: "Itinerary");
        }
    }
}
