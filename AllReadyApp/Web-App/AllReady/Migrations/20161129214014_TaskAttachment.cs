using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class TaskAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileAttachmentContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bytes = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TaskId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachment_FileAttachmentContent_ContentId",
                        column: x => x.ContentId,
                        principalTable: "FileAttachmentContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAttachment_AllReadyTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AllReadyTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_ContentId",
                table: "FileAttachment",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_TaskId",
                table: "FileAttachment",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileAttachment");

            migrationBuilder.DropTable(
                name: "FileAttachmentContent");
        }
    }
}
