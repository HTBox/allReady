using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class RenameZipToPostalCode : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Zip", "Request", "PostalCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("PostalCode", "Request", "Zip");
        }
        //protected override void Up(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropColumn(
        //        name: "Zip",
        //        table: "Request");

        //    migrationBuilder.AddColumn<string>(
        //        name: "PostalCode",
        //        table: "Request",
        //        nullable: true);
        //}

        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropColumn(
        //        name: "PostalCode",
        //        table: "Request");

        //    migrationBuilder.AddColumn<string>(
        //        name: "Zip",
        //        table: "Request",
        //        nullable: true);
        //}
    }
}
