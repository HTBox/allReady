using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddPublishedToCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Campaign",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "Campaign");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "Request",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
