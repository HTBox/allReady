using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace AllReady.Migrations
{
    public partial class InitialMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });
            migration.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    Email = table.Column(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column(type: "bit", nullable: false),
                    LockoutEnabled = table.Column(type: "bit", nullable: false),
                    LockoutEnd = table.Column(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column(type: "bit", nullable: false),
                    SecurityStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column(type: "bit", nullable: false),
                    UserName = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });
            migration.CreateTable(
                name: "PostalCodeGeo",
                columns: table => new
                {
                    PostalCode = table.Column(type: "nvarchar(450)", nullable: false),
                    City = table.Column(type: "nvarchar(max)", nullable: true),
                    State = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCodeGeo", x => x.PostalCode);
                });
            migration.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    LogoUrl = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });
            migration.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ClaimType = table.Column(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        columns: x => x.RoleId,
                        referencedTable: "AspNetRoles",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ClaimType = table.Column(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        columns: x => x.RoleId,
                        referencedTable: "AspNetRoles",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Address1 = table.Column(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column(type: "nvarchar(max)", nullable: true),
                    City = table.Column(type: "nvarchar(max)", nullable: true),
                    Country = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column(type: "nvarchar(max)", nullable: true),
                    PostalCodePostalCode = table.Column(type: "nvarchar(450)", nullable: true),
                    State = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_PostalCodeGeo_PostalCodePostalCode",
                        columns: x => x.PostalCodePostalCode,
                        referencedTable: "PostalCodeGeo",
                        referencedColumn: "PostalCode");
                });
            migration.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Description = table.Column(type: "nvarchar(max)", nullable: true),
                    EndDateTimeUtc = table.Column(type: "datetime2", nullable: false),
                    ImageUrl = table.Column(type: "nvarchar(max)", nullable: true),
                    ManagingTenantId = table.Column(type: "int", nullable: false),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    OrganizerId = table.Column(type: "nvarchar(450)", nullable: true),
                    StartDateTimeUtc = table.Column(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaign_Tenant_ManagingTenantId",
                        columns: x => x.ManagingTenantId,
                        referencedTable: "Tenant",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Campaign_ApplicationUser_OrganizerId",
                        columns: x => x.OrganizerId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    CampaignId = table.Column(type: "int", nullable: true),
                    Description = table.Column(type: "nvarchar(max)", nullable: true),
                    EndDateTimeUtc = table.Column(type: "datetime2", nullable: false),
                    LocationId = table.Column(type: "int", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    OrganizerId = table.Column(type: "nvarchar(450)", nullable: true),
                    StartDateTimeUtc = table.Column(type: "datetime2", nullable: false),
                    TenantId = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_Campaign_CampaignId",
                        columns: x => x.CampaignId,
                        referencedTable: "Campaign",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activity_Location_LocationId",
                        columns: x => x.LocationId,
                        referencedTable: "Location",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activity_ApplicationUser_OrganizerId",
                        columns: x => x.OrganizerId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activity_Tenant_TenantId",
                        columns: x => x.TenantId,
                        referencedTable: "Tenant",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "CampaignSponsors",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    CampaignId = table.Column(type: "int", nullable: true),
                    TenantId = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSponsors_Campaign_CampaignId",
                        columns: x => x.CampaignId,
                        referencedTable: "Campaign",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CampaignSponsors_Tenant_TenantId",
                        columns: x => x.TenantId,
                        referencedTable: "Tenant",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "ActivitySignup",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ActivityId = table.Column(type: "int", nullable: true),
                    CheckinDateTime = table.Column(type: "datetime2", nullable: true),
                    SignupDateTime = table.Column(type: "datetime2", nullable: false),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySignup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySignup_Activity_ActivityId",
                        columns: x => x.ActivityId,
                        referencedTable: "Activity",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivitySignup_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AllReadyTask",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ActivityId = table.Column(type: "int", nullable: true),
                    Description = table.Column(type: "nvarchar(max)", nullable: true),
                    EndDateTimeUtc = table.Column(type: "datetime2", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    StartDateTimeUtc = table.Column(type: "datetime2", nullable: true),
                    TenantId = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllReadyTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllReadyTask_Activity_ActivityId",
                        columns: x => x.ActivityId,
                        referencedTable: "Activity",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllReadyTask_Tenant_TenantId",
                        columns: x => x.TenantId,
                        referencedTable: "Tenant",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "TaskUsers",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Status = table.Column(type: "nvarchar(max)", nullable: true),
                    StatusDateTimeUtc = table.Column(type: "datetime2", nullable: false),
                    StatusDescription = table.Column(type: "nvarchar(max)", nullable: true),
                    TaskId = table.Column(type: "int", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskUsers_AllReadyTask_TaskId",
                        columns: x => x.TaskId,
                        referencedTable: "AllReadyTask",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskUsers_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("AspNetRoles");
            migration.DropTable("AspNetRoleClaims");
            migration.DropTable("AspNetUserClaims");
            migration.DropTable("AspNetUserLogins");
            migration.DropTable("AspNetUserRoles");
            migration.DropTable("Activity");
            migration.DropTable("ActivitySignup");
            migration.DropTable("AspNetUsers");
            migration.DropTable("Campaign");
            migration.DropTable("CampaignSponsors");
            migration.DropTable("Location");
            migration.DropTable("PostalCodeGeo");
            migration.DropTable("AllReadyTask");
            migration.DropTable("TaskUsers");
            migration.DropTable("Tenant");
        }
    }
}
