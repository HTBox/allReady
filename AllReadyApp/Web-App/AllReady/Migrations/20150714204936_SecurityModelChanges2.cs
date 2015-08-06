using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace AllReady.Migrations
{
    public partial class SecurityModelCHanges2 : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.AddColumn(
                name: "AssociatedTenantId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
            migration.AddForeignKey(
                name: "FK_ApplicationUser_Tenant_AssociatedTenantId",
                table: "AspNetUsers",
                column: "AssociatedTenantId",
                referencedTable: "Tenant",
                referencedColumn: "Id");
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropForeignKey(name: "FK_ApplicationUser_Tenant_AssociatedTenantId", table: "AspNetUsers");
            migration.DropColumn(name: "AssociatedTenantId", table: "AspNetUsers");
        }
    }
}
