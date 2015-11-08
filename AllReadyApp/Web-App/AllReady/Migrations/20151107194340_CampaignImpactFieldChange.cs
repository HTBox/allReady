using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace AllReady.Migrations
{
    public partial class CampaignImpactFieldChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImpactType", table: "CampaignImpactType");
            migrationBuilder.DropTable("TaskUsers");
            migrationBuilder.CreateTable(
                name: "TaskSignup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(nullable: true),
                    StatusDateTimeUtc = table.Column<DateTime>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSignup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSignup_AllReadyTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AllReadyTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskSignup_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CampaignImpactType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Name", table: "CampaignImpactType");
            migrationBuilder.DropTable("TaskSignup");
            migrationBuilder.CreateTable(
                name: "TaskUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(nullable: true),
                    StatusDateTimeUtc = table.Column<DateTime>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskUsers_AllReadyTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AllReadyTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskUsers_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
            migrationBuilder.AddColumn<string>(
                name: "ImpactType",
                table: "CampaignImpactType",
                nullable: true);
        }
    }
}
