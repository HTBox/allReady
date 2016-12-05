using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddOrgIdToRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Request",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Request_OrganizationId",
                table: "Request",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Organization_OrganizationId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_OrganizationId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Request");
        }
    }
}
