using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class RenameTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VolunteerTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsAllowWaitList = table.Column<bool>(nullable: false),
                    IsLimitVolunteers = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NumberOfVolunteersRequired = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerTask_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerTask_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerTaskSignup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    ItineraryId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusDateTimeUtc = table.Column<DateTime>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true, maxLength: 450),
                    VolunteerTaskId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTaskSignup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerTaskSignup_Itinerary_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itinerary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerTaskSignup_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerTaskSignup_VolunteerTask_VolunteerTaskId",
                        column: x => x.VolunteerTaskId,
                        principalTable: "VolunteerTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerTaskSkill",
                columns: table => new
                {
                    VolunteerTaskId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTaskSkill", x => new { x.VolunteerTaskId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_VolunteerTaskSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerTaskSkill_VolunteerTask_VolunteerTaskId",
                        column: x => x.VolunteerTaskId,
                        principalTable: "VolunteerTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTask_EventId",
                table: "VolunteerTask",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTask_OrganizationId",
                table: "VolunteerTask",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSignup_ItineraryId",
                table: "VolunteerTaskSignup",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSignup_UserId",
                table: "VolunteerTaskSignup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSignup_VolunteerTaskId",
                table: "VolunteerTaskSignup",
                column: "VolunteerTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSkill_SkillId",
                table: "VolunteerTaskSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTaskSkill_VolunteerTaskId",
                table: "VolunteerTaskSkill",
                column: "VolunteerTaskId");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT [VolunteerTask] ON
INSERT INTO [dbo].[VolunteerTask]([Id],[Description],[EndDateTime],[EventId],[IsAllowWaitList],[IsLimitVolunteers],[Name],[NumberOfVolunteersRequired],[OrganizationId],[StartDateTime])
SELECT [Id],[Description],[EndDateTime],[EventId],[IsAllowWaitList],[IsLimitVolunteers],[Name],[NumberOfVolunteersRequired],[OrganizationId],[StartDateTime]
FROM [dbo].[AllReadyTask]
SET IDENTITY_INSERT [VolunteerTask] OFF");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT [VolunteerTaskSignup] ON
INSERT INTO [dbo].[VolunteerTaskSignup] ([Id],[AdditionalInfo],[ItineraryId],[Status],[StatusDateTimeUtc],[StatusDescription],[VolunteerTaskId],[UserId])
SELECT [Id],[AdditionalInfo],[ItineraryId],[Status],[StatusDateTimeUtc],[StatusDescription],[TaskId],[UserId]
FROM [dbo].[TaskSignup]
SET IDENTITY_INSERT [VolunteerTaskSignup] OFF");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[VolunteerTaskSkill]([VolunteerTaskId],[SkillId])
SELECT [TaskId],[SkillId]
FROM [dbo].[TaskSkill]");

            migrationBuilder.DropTable(
                name: "TaskSignup");

            migrationBuilder.DropTable(
                name: "TaskSkill");

            migrationBuilder.DropTable(
                name: "AllReadyTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllReadyTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsAllowWaitList = table.Column<bool>(nullable: false),
                    IsLimitVolunteers = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NumberOfVolunteersRequired = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllReadyTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllReadyTask_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllReadyTask_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskSignup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    ItineraryId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusDateTimeUtc = table.Column<DateTime>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true, maxLength: 450)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSignup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSignup_Itinerary_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itinerary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskSignup_AllReadyTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AllReadyTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSignup_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskSkill",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSkill", x => new { x.TaskId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_TaskSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSkill_AllReadyTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AllReadyTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllReadyTask_EventId",
                table: "AllReadyTask",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AllReadyTask_OrganizationId",
                table: "AllReadyTask",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSignup_ItineraryId",
                table: "TaskSignup",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSignup_TaskId",
                table: "TaskSignup",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSignup_UserId",
                table: "TaskSignup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSkill_SkillId",
                table: "TaskSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSkill_TaskId",
                table: "TaskSkill",
                column: "TaskId");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT [AllReadyTask] ON
INSERT INTO [dbo].[AllReadyTask]([Id],[Description],[EndDateTime],[EventId],[IsAllowWaitList],[IsLimitVolunteers],[Name],[NumberOfVolunteersRequired],[OrganizationId],[StartDateTime])
SELECT [Id],[Description],[EndDateTime],[EventId],[IsAllowWaitList],[IsLimitVolunteers],[Name],[NumberOfVolunteersRequired],[OrganizationId],[StartDateTime]
FROM [dbo].[VolunteerTask]
SET IDENTITY_INSERT [AllReadyTask] OFF");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT [TaskSignup] ON
INSERT INTO [dbo].[TaskSignup] ([Id],[AdditionalInfo],[ItineraryId],[Status],[StatusDateTimeUtc],[StatusDescription],[TaskId],[UserId])
SELECT [Id],[AdditionalInfo],[ItineraryId],[Status],[StatusDateTimeUtc],[StatusDescription],[VolunteerTaskId],[UserId]
FROM [dbo].[VolunteerTaskSignup]
SET IDENTITY_INSERT [TaskSignup] OFF");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[TaskSkill]([TaskId],[SkillId])
SELECT [VolunteerTaskId],[SkillId]
FROM [dbo].[VolunteerTaskSkill]");

            migrationBuilder.DropTable(
                name: "VolunteerTaskSignup");

            migrationBuilder.DropTable(
                name: "VolunteerTaskSkill");

            migrationBuilder.DropTable(
                name: "VolunteerTask");
        }
    }
}
