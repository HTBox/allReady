using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class RemoveUserTenantRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_Tenant_AssociatedTenantId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AssociatedTenantId", table: "AspNetUsers");
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Tenant_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_Tenant_TenantId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "TenantId", table: "AspNetUsers");
            migrationBuilder.AddColumn<int>(
                name: "AssociatedTenantId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Tenant_AssociatedTenantId",
                table: "AspNetUsers",
                column: "AssociatedTenantId",
                principalTable: "Tenant",
                principalColumn: "Id");
        }
    }
}
