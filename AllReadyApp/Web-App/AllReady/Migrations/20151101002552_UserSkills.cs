using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class UserSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSkill",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkill", x => new { x.UserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_UserSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSkill_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("UserSkill");
        }
    }
}
