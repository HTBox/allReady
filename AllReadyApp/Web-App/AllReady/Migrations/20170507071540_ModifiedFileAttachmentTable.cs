using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class ModifiedFileAttachmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachment_VolunteerTask_TaskId",
                table: "FileAttachment");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachment_TaskId",
                table: "FileAttachment");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "FileAttachment",
                newName: "VolunteerTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_VolunteerTaskId",
                table: "FileAttachment",
                column: "VolunteerTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachment_VolunteerTask_VolunteerTaskId",
                table: "FileAttachment",
                column: "VolunteerTaskId",
                principalTable: "VolunteerTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachment_VolunteerTask_VolunteerTaskId",
                table: "FileAttachment");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachment_VolunteerTaskId",
                table: "FileAttachment");

            migrationBuilder.RenameColumn(
                name: "VolunteerTaskId",
                table: "FileAttachment",
                newName: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_TaskId",
                table: "FileAttachment",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachment_VolunteerTask_TaskId",
                table: "FileAttachment",
                column: "TaskId",
                principalTable: "VolunteerTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
