using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class SkillDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Skill",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Description", table: "Skill");
        }
    }
}
