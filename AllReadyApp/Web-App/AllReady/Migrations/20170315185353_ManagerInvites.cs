using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class ManagerInvites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignManagerInvite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AcceptedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    CampaignId = table.Column<int>(nullable: false),
                    CustomMessage = table.Column<string>(nullable: true),
                    InviteeEmailAddress = table.Column<string>(nullable: false),
                    RejectedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    RevokedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    SenderUserId = table.Column<string>(nullable: false),
                    SentDateTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignManagerInvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignManagerInvite_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignManagerInvite_ApplicationUser_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventManagerInvite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AcceptedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    CustomMessage = table.Column<string>(nullable: true),
                    EventId = table.Column<int>(nullable: false),
                    InviteeEmailAddress = table.Column<string>(nullable: false),
                    RejectedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    RevokedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    SenderUserId = table.Column<string>(nullable: false),
                    SentDateTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagerInvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventManagerInvite_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventManagerInvite_ApplicationUser_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignManagerInvite_CampaignId",
                table: "CampaignManagerInvite",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignManagerInvite_SenderUserId",
                table: "CampaignManagerInvite",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManagerInvite_EventId",
                table: "EventManagerInvite",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManagerInvite_SenderUserId",
                table: "EventManagerInvite",
                column: "SenderUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignManagerInvite");

            migrationBuilder.DropTable(
                name: "EventManagerInvite");
        }
    }
}
