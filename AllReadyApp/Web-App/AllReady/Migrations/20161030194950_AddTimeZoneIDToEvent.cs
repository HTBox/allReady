using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddTimeZoneIDToEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Event",
                nullable: false,
                defaultValue: "Central Standard Time");

            //Set the TimeZoneID for existing events based on the TimeZoneID of the parent campaign
            migrationBuilder.Sql(
@"UPDATE e SET e.[TimeZoneID] = c.[TimeZoneID]
  FROM [Event] e
       INNER JOIN Campaign c ON c.Id = e.CampaignId
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Event");
        }
    }
}
