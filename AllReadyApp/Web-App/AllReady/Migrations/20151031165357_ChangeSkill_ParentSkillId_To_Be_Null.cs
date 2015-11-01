using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class ChangeSkill_ParentSkillId_To_Be_Null : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ParentSkillId",
                table: "Skill",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ParentSkillId",
                table: "Skill",
                nullable: false);
        }
    }
}
