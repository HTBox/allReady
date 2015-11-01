using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class SkillNameRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skill",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skill",
                nullable: true);
        }
    }
}
