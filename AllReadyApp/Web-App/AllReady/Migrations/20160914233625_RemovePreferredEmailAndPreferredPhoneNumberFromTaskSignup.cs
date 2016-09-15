using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class RemovePreferredEmailAndPreferredPhoneNumberFromTaskSignup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredEmail",
                table: "TaskSignup");

            migrationBuilder.DropColumn(
                name: "PreferredPhoneNumber",
                table: "TaskSignup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredEmail",
                table: "TaskSignup",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredPhoneNumber",
                table: "TaskSignup",
                nullable: true);
        }
    }
}
