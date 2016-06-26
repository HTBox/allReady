using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using AllReady.Models;

namespace AllReady.Migrations
{
    [DbContext(typeof(AllReadyContext))]
    partial class AllReadyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("EndDateTime");

                    b.Property<int?>("EventId");

                    b.Property<bool>("IsAllowWaitList");

                    b.Property<bool>("IsLimitVolunteers");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfVolunteersRequired");

                    b.Property<int?>("OrganizationId");

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.HasKey("Id");
                });

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
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
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

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.Property<string>("TimeZoneId")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignContact", b =>
                {
                    b.Property<int>("CampaignId");

                    b.Property<int>("ContactId");

                    b.Property<int>("ContactType");

                    b.HasKey("CampaignId", "ContactId", "ContactType");
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
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CampaignId");

                    b.Property<int?>("OrganizationId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.ClosestLocation", b =>
                {
                    b.Property<string>("PostalCode");

                    b.Property<string>("City");

                    b.Property<double>("Distance");

                    b.Property<string>("State");

                    b.HasKey("PostalCode");
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

                    b.Property<int>("NumberOfVolunteersRequired");

                    b.Property<string>("OrganizerId");

                    b.Property<DateTimeOffset>("StartDateTime");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.EventSignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInfo");

                    b.Property<DateTime?>("CheckinDateTime");

                    b.Property<int?>("EventId");

                    b.Property<string>("PreferredEmail");

                    b.Property<string>("PreferredPhoneNumber");

                    b.Property<DateTime>("SignupDateTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.EventSkill", b =>
                {
                    b.Property<int>("EventId");

                    b.Property<int>("SkillId");

                    b.HasKey("EventId", "SkillId");
                });

            modelBuilder.Entity("AllReady.Models.Itinerary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("EventId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.ItineraryRequest", b =>
                {
                    b.Property<int>("ItineraryId");

                    b.Property<Guid>("RequestId");

                    b.Property<DateTime>("DateAssigned");

                    b.Property<int>("OrderIndex");

                    b.HasKey("ItineraryId", "RequestId");
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

                    b.Property<string>("Summary")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("WebUrl");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.OrganizationContact", b =>
                {
                    b.Property<int>("OrganizationId");

                    b.Property<int>("ContactId");

                    b.Property<int>("ContactType");

                    b.HasKey("OrganizationId", "ContactId", "ContactType");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeo", b =>
                {
                    b.Property<string>("PostalCode");

                    b.Property<string>("City");

                    b.Property<string>("State");

                    b.HasKey("PostalCode");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeoCoordinate", b =>
                {
                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.HasKey("Latitude", "Longitude");
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

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("ProviderData");

                    b.Property<string>("ProviderId");

                    b.Property<string>("State");

                    b.Property<int>("Status");

                    b.Property<string>("Zip");

                    b.HasKey("RequestId");
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
                });

            modelBuilder.Entity("AllReady.Models.TaskSignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInfo");

                    b.Property<int?>("ItineraryId");

                    b.Property<string>("PreferredEmail");

                    b.Property<string>("PreferredPhoneNumber");

                    b.Property<string>("Status");

                    b.Property<DateTime>("StatusDateTimeUtc");

                    b.Property<string>("StatusDescription");

                    b.Property<int>("TaskId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.TaskSkill", b =>
                {
                    b.Property<int>("TaskId");

                    b.Property<int>("SkillId");

                    b.HasKey("TaskId", "SkillId");
                });

            modelBuilder.Entity("AllReady.Models.UserSkill", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("SkillId");

                    b.HasKey("UserId", "SkillId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
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
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.HasOne("AllReady.Models.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.HasOne("AllReady.Models.CampaignImpact")
                        .WithMany()
                        .HasForeignKey("CampaignImpactId");

                    b.HasOne("AllReady.Models.Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("ManagingOrganizationId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.CampaignContact", b =>
                {
                    b.HasOne("AllReady.Models.Campaign")
                        .WithMany()
                        .HasForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.HasOne("AllReady.Models.Campaign")
                        .WithMany()
                        .HasForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Event", b =>
                {
                    b.HasOne("AllReady.Models.Campaign")
                        .WithMany()
                        .HasForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.EventSignup", b =>
                {
                    b.HasOne("AllReady.Models.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AllReady.Models.EventSkill", b =>
                {
                    b.HasOne("AllReady.Models.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");
                });

            modelBuilder.Entity("AllReady.Models.Itinerary", b =>
                {
                    b.HasOne("AllReady.Models.Event")
                        .WithMany()
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("AllReady.Models.ItineraryRequest", b =>
                {
                    b.HasOne("AllReady.Models.Itinerary")
                        .WithMany()
                        .HasForeignKey("ItineraryId");

                    b.HasOne("AllReady.Models.Request")
                        .WithMany()
                        .HasForeignKey("RequestId");
                });

            modelBuilder.Entity("AllReady.Models.Organization", b =>
                {
                    b.HasOne("AllReady.Models.Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("AllReady.Models.OrganizationContact", b =>
                {
                    b.HasOne("AllReady.Models.Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");

                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("AllReady.Models.Request", b =>
                {
                    b.HasOne("AllReady.Models.Event")
                        .WithMany()
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("AllReady.Models.Skill", b =>
                {
                    b.HasOne("AllReady.Models.Organization")
                        .WithMany()
                        .HasForeignKey("OwningOrganizationId");

                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .HasForeignKey("ParentSkillId");
                });

            modelBuilder.Entity("AllReady.Models.TaskSignup", b =>
                {
                    b.HasOne("AllReady.Models.Itinerary")
                        .WithMany()
                        .HasForeignKey("ItineraryId");

                    b.HasOne("AllReady.Models.AllReadyTask")
                        .WithMany()
                        .HasForeignKey("TaskId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AllReady.Models.TaskSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");

                    b.HasOne("AllReady.Models.AllReadyTask")
                        .WithMany()
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("AllReady.Models.UserSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .HasForeignKey("SkillId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
