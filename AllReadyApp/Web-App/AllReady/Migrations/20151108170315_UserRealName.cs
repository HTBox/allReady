using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class UserRealName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "ActivitySignup",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PreferredEmail",
                table: "ActivitySignup",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PreferredPhoneNumber",
                table: "ActivitySignup",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Name", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AdditionalInfo", table: "ActivitySignup");
            migrationBuilder.DropColumn(name: "PreferredEmail", table: "ActivitySignup");
            migrationBuilder.DropColumn(name: "PreferredPhoneNumber", table: "ActivitySignup");
        }
    }
}
