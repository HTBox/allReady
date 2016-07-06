using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllReady.Migrations
{
    public partial class InitialRTM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignImpact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentImpactLevel = table.Column<int>(nullable: false),
                    Display = table.Column<bool>(nullable: false),
                    ImpactType = table.Column<int>(nullable: false),
                    NumericImpactGoal = table.Column<int>(nullable: false),
                    TextualImpactGoal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignImpact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClosestLocation",
                columns: table => new
                {
                    PostalCode = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    Distance = table.Column<double>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosestLocation", x => x.PostalCode);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostalCodeGeo",
                columns: table => new
                {
                    PostalCode = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCodeGeo", x => x.PostalCode);
                });

            migrationBuilder.CreateTable(
                name: "PostalCodeGeoCoordinate",
                columns: table => new
                {
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCodeGeoCoordinate", x => new { x.Latitude, x.Longitude });
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryTag = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MediaUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PublishDateBegin = table.Column<DateTime>(nullable: false),
                    PublishDateEnd = table.Column<DateTime>(nullable: false),
                    ResourceUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserToken<string>",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserToken<string>", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionHtml = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PrivacyPolicy = table.Column<string>(nullable: true),
                    PrivacyPolicyUrl = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(maxLength: 250, nullable: true),
                    WebUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaim<string>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    OrganizationId = table.Column<int>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PendingNewEmail = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TimeZoneId = table.Column<string>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationContact",
                columns: table => new
                {
                    OrganizationId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactType = table.Column<int>(nullable: false),
                    ContactId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationContact", x => new { x.OrganizationId, x.ContactId, x.ContactType });
                    table.ForeignKey(
                        name: "FK_OrganizationContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationContact_Contact_ContactId1",
                        column: x => x.ContactId1,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationContact_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    OwningOrganizationId = table.Column<int>(nullable: true),
                    ParentSkillId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_Organization_OwningOrganizationId",
                        column: x => x.OwningOrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Skill_ParentSkillId",
                        column: x => x.ParentSkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignImpactId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: false),
                    ExternalUrl = table.Column<string>(nullable: true),
                    ExternalUrlText = table.Column<string>(nullable: true),
                    Featured = table.Column<bool>(nullable: false),
                    FullDescription = table.Column<string>(nullable: true),
                    Headline = table.Column<string>(maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    ManagingOrganizationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OrganizerId = table.Column<string>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: false),
                    TimeZoneId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaign_CampaignImpact_CampaignImpactId",
                        column: x => x.CampaignImpactId,
                        principalTable: "CampaignImpact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaign_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaign_Organization_ManagingOrganizationId",
                        column: x => x.ManagingOrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campaign_ApplicationUser_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaim<string>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin<string>",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRole<string>",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSkill",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkill", x => new { x.UserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_UserSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSkill_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignContact",
                columns: table => new
                {
                    CampaignId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactType = table.Column<int>(nullable: false),
                    ContactId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignContact", x => new { x.CampaignId, x.ContactId, x.ContactType });
                    table.ForeignKey(
                        name: "FK_CampaignContact_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignContact_Contact_ContactId1",
                        column: x => x.ContactId1,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSponsors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<int>(nullable: true),
                    OrganizationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignSponsors_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignSponsors_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    Headline = table.Column<string>(maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    IsAllowWaitList = table.Column<bool>(nullable: false),
                    IsLimitVolunteers = table.Column<bool>(nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NumberOfVolunteersRequired = table.Column<int>(nullable: false),
                    OrganizerId = table.Column<string>(nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Event_ApplicationUser_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllReadyTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(nullable: false),
                    EventId = table.Column<int>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "EventSkill",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSkill", x => new { x.EventId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EventSkill_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Itinerary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itinerary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Itinerary_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EventId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    ProviderData = table.Column<string>(nullable: true),
                    ProviderId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Request_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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

            migrationBuilder.CreateTable(
                name: "TaskSignup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    ItineraryId = table.Column<int>(nullable: true),
                    PreferredEmail = table.Column<string>(nullable: true),
                    PreferredPhoneNumber = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusDateTimeUtc = table.Column<DateTime>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
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
                name: "ItineraryRequest",
                columns: table => new
                {
                    ItineraryId = table.Column<int>(nullable: false),
                    RequestId = table.Column<Guid>(nullable: false),
                    DateAssigned = table.Column<DateTime>(nullable: false),
                    OrderIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItineraryRequest", x => new { x.ItineraryId, x.RequestId });
                    table.ForeignKey(
                        name: "FK_ItineraryRequest_Itinerary_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itinerary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItineraryRequest_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "RequestId",
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
                name: "EmailIndex",
                table: "ApplicationUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_OrganizationId",
                table: "ApplicationUser",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_CampaignImpactId",
                table: "Campaign",
                column: "CampaignImpactId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_LocationId",
                table: "Campaign",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_ManagingOrganizationId",
                table: "Campaign",
                column: "ManagingOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_OrganizerId",
                table: "Campaign",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContact_CampaignId",
                table: "CampaignContact",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContact_ContactId",
                table: "CampaignContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignContact_ContactId1",
                table: "CampaignContact",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSponsors_CampaignId",
                table: "CampaignSponsors",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSponsors_OrganizationId",
                table: "CampaignSponsors",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CampaignId",
                table: "Event",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_LocationId",
                table: "Event",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerId",
                table: "Event",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSignup_EventId",
                table: "EventSignup",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSignup_UserId",
                table: "EventSignup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSkill_EventId",
                table: "EventSkill",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSkill_SkillId",
                table: "EventSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Itinerary_EventId",
                table: "Itinerary",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_ItineraryId",
                table: "ItineraryRequest",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryRequest_RequestId",
                table: "ItineraryRequest",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_LocationId",
                table: "Organization",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContact_ContactId",
                table: "OrganizationContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContact_ContactId1",
                table: "OrganizationContact",
                column: "ContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContact_OrganizationId",
                table: "OrganizationContact",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_EventId",
                table: "Request",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_OwningOrganizationId",
                table: "Skill",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_ParentSkillId",
                table: "Skill",
                column: "ParentSkillId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserSkill_SkillId",
                table: "UserSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkill_UserId",
                table: "UserSkill",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityRole",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoleClaim<string>_RoleId",
                table: "IdentityRoleClaim<string>",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaim<string>_UserId",
                table: "IdentityUserClaim<string>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogin<string>_UserId",
                table: "IdentityUserLogin<string>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole<string>_RoleId",
                table: "IdentityUserRole<string>",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole<string>_UserId",
                table: "IdentityUserRole<string>",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignContact");

            migrationBuilder.DropTable(
                name: "CampaignSponsors");

            migrationBuilder.DropTable(
                name: "ClosestLocation");

            migrationBuilder.DropTable(
                name: "EventSignup");

            migrationBuilder.DropTable(
                name: "EventSkill");

            migrationBuilder.DropTable(
                name: "ItineraryRequest");

            migrationBuilder.DropTable(
                name: "OrganizationContact");

            migrationBuilder.DropTable(
                name: "PostalCodeGeo");

            migrationBuilder.DropTable(
                name: "PostalCodeGeoCoordinate");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "TaskSignup");

            migrationBuilder.DropTable(
                name: "TaskSkill");

            migrationBuilder.DropTable(
                name: "UserSkill");

            migrationBuilder.DropTable(
                name: "IdentityRoleClaim<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserClaim<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserLogin<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserRole<string>");

            migrationBuilder.DropTable(
                name: "IdentityUserToken<string>");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Itinerary");

            migrationBuilder.DropTable(
                name: "AllReadyTask");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropTable(
                name: "CampaignImpact");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
