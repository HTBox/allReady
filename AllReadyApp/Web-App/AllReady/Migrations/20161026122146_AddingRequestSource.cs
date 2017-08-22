using System;
using System.Collections.Generic;
using AllReady.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddingRequestSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Request",
                nullable: false,
                defaultValue: (int)RequestSource.Unknown);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Request");
        }
    }
}
