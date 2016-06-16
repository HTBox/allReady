using System;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class ItineraryRequestChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Request",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAssigned",
                table: "ItineraryRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "ItineraryRequest",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DateAdded", table: "Request");
            migrationBuilder.DropColumn(name: "DateAssigned", table: "ItineraryRequest");
            migrationBuilder.DropColumn(name: "OrderIndex", table: "ItineraryRequest");
        }
    }
}
