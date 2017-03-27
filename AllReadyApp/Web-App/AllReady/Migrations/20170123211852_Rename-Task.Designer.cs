using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AllReady.Models;

namespace AllReady.Migrations
{
    [DbContext(typeof(AllReadyContext))]
    [Migration("20170123211852_Rename-Task")]
    partial class RenameTask
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<int?>("OrganizationId");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PendingNewEmail");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("TimeZoneId")
                        .IsRequired();

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("OrganizationId");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CampaignImpactId");

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("EndDateTime");

                    b.Property<string>("ExternalUrl");

                    b.Property<string>("ExternalUrlText");

                    b.Property<bool>("Featured");

                    b.Property<string>("FullDescription");

                    b.Property<string>("Headline")
                        .HasAnnotation("MaxLength", 150);

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("LocationId");

                    b.Property<bool>("Locked");

                    b.Property<int>("ManagingOrganizationId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OrganizerId");

                    b.Property<bool>("Published");

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.Property<string>("TimeZoneId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CampaignImpactId");

                    b.HasIndex("LocationId");

                    b.HasIndex("ManagingOrganizationId");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Campaign");
                });

            modelBuilder.Entity("AllReady.Models.CampaignContact", b =>
                {
                    b.Property<int>("CampaignId");

                    b.Property<int>("ContactId");

                    b.Property<int>("ContactType");

                    b.Property<int?>("ContactId1");

                    b.HasKey("CampaignId", "ContactId", "ContactType");

                    b.HasIndex("CampaignId");

                    b.HasIndex("ContactId");

                    b.HasIndex("ContactId1");

                    b.ToTable("CampaignContact");
                });

            modelBuilder.Entity("AllReady.Models.CampaignImpact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentImpactLevel");

                    b.Property<bool>("Display");

                    b.Property<int>("ImpactType");

                    b.Property<int>("NumericImpactGoal");

                    b.Property<string>("TextualImpactGoal");

                    b.HasKey("Id");

                    b.ToTable("CampaignImpact");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CampaignId");

                    b.Property<int?>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("CampaignSponsors");
                });

            modelBuilder.Entity("AllReady.Models.ClosestLocation", b =>
                {
                    b.Property<string>("PostalCode");

                    b.Property<string>("City");

                    b.Property<double>("Distance");

                    b.Property<string>("State");

                    b.HasKey("PostalCode");

                    b.ToTable("ClosestLocation");
                });

            modelBuilder.Entity("AllReady.Models.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("PhoneNumber");

                    b.HasKey("Id");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("AllReady.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CampaignId");

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("EndDateTime");

                    b.Property<int>("EventType");

                    b.Property<string>("Headline")
                        .HasAnnotation("MaxLength", 150);

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsAllowWaitList");

                    b.Property<bool>("IsLimitVolunteers");

                    b.Property<int?>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OrganizerId");

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.Property<string>("TimeZoneId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.HasIndex("LocationId");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("AllReady.Models.EventSkill", b =>
                {
                    b.Property<int>("EventId");

                    b.Property<int>("SkillId");

                    b.HasKey("EventId", "SkillId");

                    b.HasIndex("EventId");

                    b.HasIndex("SkillId");

                    b.ToTable("EventSkill");
                });

            modelBuilder.Entity("AllReady.Models.Itinerary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<double>("EndLatitude");

                    b.Property<int?>("EndLocationId");

                    b.Property<double>("EndLongitude");

                    b.Property<int>("EventId");

                    b.Property<string>("Name");

                    b.Property<double>("StartLatitude");

                    b.Property<int?>("StartLocationId");

                    b.Property<double>("StartLongitude");

                    b.Property<bool>("UseStartAddressAsEndAddress");

                    b.HasKey("Id");

                    b.HasIndex("EndLocationId");

                    b.HasIndex("EventId");

                    b.HasIndex("StartLocationId");

                    b.ToTable("Itinerary");
                });

            modelBuilder.Entity("AllReady.Models.ItineraryRequest", b =>
                {
                    b.Property<int>("ItineraryId");

                    b.Property<Guid>("RequestId");

                    b.Property<DateTime>("DateAssigned");

                    b.Property<int>("OrderIndex");

                    b.HasKey("ItineraryId", "RequestId");

                    b.HasIndex("ItineraryId");

                    b.HasIndex("RequestId")
                        .IsUnique();

                    b.ToTable("ItineraryRequest");
                });

            modelBuilder.Entity("AllReady.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostalCode");

                    b.Property<string>("State");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("AllReady.Models.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DescriptionHtml");

                    b.Property<int?>("LocationId");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PrivacyPolicy");

                    b.Property<string>("PrivacyPolicyUrl");

                    b.Property<string>("Summary")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("WebUrl");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("AllReady.Models.OrganizationContact", b =>
                {
                    b.Property<int>("OrganizationId");

                    b.Property<int>("ContactId");

                    b.Property<int>("ContactType");

                    b.Property<int?>("ContactId1");

                    b.HasKey("OrganizationId", "ContactId", "ContactType");

                    b.HasIndex("ContactId");

                    b.HasIndex("ContactId1");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationContact");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeo", b =>
                {
                    b.Property<string>("PostalCode");

                    b.Property<string>("City");

                    b.Property<string>("State");

                    b.HasKey("PostalCode");

                    b.ToTable("PostalCodeGeo");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeoCoordinate", b =>
                {
                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.HasKey("Latitude", "Longitude");

                    b.ToTable("PostalCodeGeoCoordinate");
                });

            modelBuilder.Entity("AllReady.Models.Request", b =>
                {
                    b.Property<Guid>("RequestId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Email");

                    b.Property<int?>("EventId");

                    b.Property<int?>("ItineraryId");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name");

                    b.Property<int?>("OrganizationId");

                    b.Property<string>("Phone");

                    b.Property<string>("ProviderData");

                    b.Property<string>("ProviderRequestId");

                    b.Property<int>("Source");

                    b.Property<string>("State");

                    b.Property<int>("Status");

                    b.Property<string>("Zip");

                    b.HasKey("RequestId");

                    b.HasIndex("EventId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("AllReady.Models.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryTag");

                    b.Property<string>("Description");

                    b.Property<string>("MediaUrl");

                    b.Property<string>("Name");

                    b.Property<DateTime>("PublishDateBegin");

                    b.Property<DateTime>("PublishDateEnd");

                    b.Property<string>("ResourceUrl");

                    b.HasKey("Id");

                    b.ToTable("Resource");
                });

            modelBuilder.Entity("AllReady.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("OwningOrganizationId");

                    b.Property<int?>("ParentSkillId");

                    b.HasKey("Id");

                    b.HasIndex("OwningOrganizationId");

                    b.HasIndex("ParentSkillId");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("AllReady.Models.UserSkill", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("SkillId");

                    b.HasKey("UserId", "SkillId");

                    b.HasIndex("SkillId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSkill");
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("EndDateTime");

                    b.Property<int>("EventId");

                    b.Property<bool>("IsAllowWaitList");

                    b.Property<bool>("IsLimitVolunteers");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfVolunteersRequired");

                    b.Property<int?>("OrganizationId");

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("VolunteerTask");
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTaskSignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInfo");

                    b.Property<int?>("ItineraryId");

                    b.Property<int>("Status");

                    b.Property<DateTime>("StatusDateTimeUtc");

                    b.Property<string>("StatusDescription");

                    b.Property<string>("UserId");

                    b.Property<int>("VolunteerTaskId");

                    b.HasKey("Id");

                    b.HasIndex("ItineraryId");

                    b.HasIndex("UserId");

                    b.HasIndex("VolunteerTaskId");

                    b.ToTable("VolunteerTaskSignup");
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTaskSkill", b =>
                {
                    b.Property<int>("VolunteerTaskId");

                    b.Property<int>("SkillId");

                    b.HasKey("VolunteerTaskId", "SkillId");

                    b.HasIndex("SkillId");

                    b.HasIndex("VolunteerTaskId");

                    b.ToTable("VolunteerTaskSkill");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("IdentityRoleClaim<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserClaim<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserLogin<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserRole<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("IdentityUserToken<string>");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.HasOne("AllReady.Models.Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.HasOne("AllReady.Models.CampaignImpact", "CampaignImpact")
                        .WithMany()
                        .HasForeignKey("CampaignImpactId");

                    b.HasOne("AllReady.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("AllReady.Models.Organization", "ManagingOrganization")
                        .WithMany("Campaigns")
                        .HasForeignKey("ManagingOrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.ApplicationUser", "Organizer")
                        .WithMany()
                        .HasForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.CampaignContact", b =>
                {
                    b.HasOne("AllReady.Models.Campaign", "Campaign")
                        .WithMany("CampaignContacts")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Contact")
                        .WithMany("CampaignContacts")
                        .HasForeignKey("ContactId1");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.HasOne("AllReady.Models.Campaign", "Campaign")
                        .WithMany("ParticipatingOrganizations")
                        .HasForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Event", b =>
                {
                    b.HasOne("AllReady.Models.Campaign", "Campaign")
                        .WithMany("Events")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("AllReady.Models.ApplicationUser", "Organizer")
                        .WithMany()
                        .HasForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.EventSkill", b =>
                {
                    b.HasOne("AllReady.Models.Event", "Event")
                        .WithMany("RequiredSkills")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AllReady.Models.Itinerary", b =>
                {
                    b.HasOne("AllReady.Models.Location", "EndLocation")
                        .WithMany()
                        .HasForeignKey("EndLocationId");

                    b.HasOne("AllReady.Models.Event", "Event")
                        .WithMany("Itineraries")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Location", "StartLocation")
                        .WithMany()
                        .HasForeignKey("StartLocationId");
                });

            modelBuilder.Entity("AllReady.Models.ItineraryRequest", b =>
                {
                    b.HasOne("AllReady.Models.Itinerary", "Itinerary")
                        .WithMany("Requests")
                        .HasForeignKey("ItineraryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Request", "Request")
                        .WithOne("Itinerary")
                        .HasForeignKey("AllReady.Models.ItineraryRequest", "RequestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AllReady.Models.Organization", b =>
                {
                    b.HasOne("AllReady.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("AllReady.Models.OrganizationContact", b =>
                {
                    b.HasOne("AllReady.Models.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Contact")
                        .WithMany("OrganizationContacts")
                        .HasForeignKey("ContactId1");

                    b.HasOne("AllReady.Models.Organization", "Organization")
                        .WithMany("OrganizationContacts")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AllReady.Models.Request", b =>
                {
                    b.HasOne("AllReady.Models.Event", "Event")
                        .WithMany("Requests")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("AllReady.Models.Organization", "Organization")
                        .WithMany("Requests")
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Skill", b =>
                {
                    b.HasOne("AllReady.Models.Organization", "OwningOrganization")
                        .WithMany()
                        .HasForeignKey("OwningOrganizationId");

                    b.HasOne("AllReady.Models.Skill", "ParentSkill")
                        .WithMany("ChildSkills")
                        .HasForeignKey("ParentSkillId");
                });

            modelBuilder.Entity("AllReady.Models.UserSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.ApplicationUser", "User")
                        .WithMany("AssociatedSkills")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTask", b =>
                {
                    b.HasOne("AllReady.Models.Event", "Event")
                        .WithMany("Tasks")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTaskSignup", b =>
                {
                    b.HasOne("AllReady.Models.Itinerary", "Itinerary")
                        .WithMany("TeamMembers")
                        .HasForeignKey("ItineraryId");

                    b.HasOne("AllReady.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.HasOne("AllReady.Models.VolunteerTask", "VolunteerTask")
                        .WithMany("AssignedVolunteers")
                        .HasForeignKey("VolunteerTaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AllReady.Models.VolunteerTaskSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.VolunteerTask", "VolunteerTask")
                        .WithMany("RequiredSkills")
                        .HasForeignKey("VolunteerTaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
