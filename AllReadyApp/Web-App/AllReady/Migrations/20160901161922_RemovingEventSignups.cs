using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class RemovingEventSignups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventSignup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventSignup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    CheckinDateTime = table.Column<DateTime>(nullable: true),
                    EventId = table.Column<int>(nullable: true),
                    PreferredEmail = table.Column<string>(nullable: true),
                    PreferredPhoneNumber = table.Column<string>(nullable: true),
                    SignupDateTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSignup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSignup_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSignup_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSignup_EventId",
                table: "EventSignup",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSignup_UserId",
                table: "EventSignup",
                column: "UserId");
        }
    }
}
