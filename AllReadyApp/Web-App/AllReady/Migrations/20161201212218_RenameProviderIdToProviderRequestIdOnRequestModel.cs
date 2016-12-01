using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class RenameProviderIdToProviderRequestIdOnRequestModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Request");

            migrationBuilder.AddColumn<string>(
                name: "ProviderRequestId",
                table: "Request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderRequestId",
                table: "Request");

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Request",
                nullable: true);
        }
    }
}
