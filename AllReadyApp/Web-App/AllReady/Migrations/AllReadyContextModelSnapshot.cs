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
                .Annotation("ProductVersion", "7.0.0-beta8-15964")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AllReady.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CampaignId");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDateTimeUtc");

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("LocationId");

                    b.Property<string>("Name");

                    b.Property<int>("NumberOfVolunteersRequired");

                    b.Property<string>("OrganizerId");

                    b.Property<DateTime>("StartDateTimeUtc");

                    b.Property<int>("TenantId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<DateTime?>("CheckinDateTime");

                    b.Property<DateTime>("SignupDateTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySkill", b =>
                {
                    b.Property<int>("ActivityId");

                    b.Property<int>("SkillId");

                    b.HasKey("ActivityId", "SkillId");
                });

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset?>("EndDateTimeUtc");

                    b.Property<string>("Name");

                    b.Property<int>("NumberOfVolunteersRequired");

                    b.Property<DateTimeOffset?>("StartDateTimeUtc");

                    b.Property<int?>("TenantId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .Annotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<int?>("TenantId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .Annotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.Index("NormalizedEmail")
                        .Annotation("Relational:Name", "EmailIndex");

                    b.Index("NormalizedUserName")
                        .Annotation("Relational:Name", "UserNameIndex");

                    b.Annotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDateTimeUtc");

                    b.Property<string>("FullDescription");

                    b.Property<string>("ImageUrl");

                    b.Property<int>("ManagingTenantId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OrganizerId");

                    b.Property<DateTime>("StartDateTimeUtc");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignImpact", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("CampaignImpactTypeId");

                    b.Property<int>("CurrentImpactLevel");

                    b.Property<bool>("Display");

                    b.Property<int>("NumericImpactGoal");

                    b.Property<string>("TextualImpactGoal");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignImpactType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CampaignId");

                    b.Property<int?>("TenantId");

                    b.HasKey("Id");
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

                    b.Property<string>("PostalCodePostalCode");

                    b.Property<string>("State");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeo", b =>
                {
                    b.Property<string>("PostalCode");

                    b.Property<string>("City");

                    b.Property<string>("State");

                    b.HasKey("PostalCode");
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

                    b.Property<int?>("ParentSkillId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.TaskSignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Status");

                    b.Property<DateTime>("StatusDateTimeUtc");

                    b.Property<string>("StatusDescription");

                    b.Property<int?>("TaskId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("AllReady.Models.TaskSkill", b =>
                {
                    b.Property<int>("TaskId");

                    b.Property<int>("SkillId");

                    b.HasKey("TaskId", "SkillId");
                });

            modelBuilder.Entity("AllReady.Models.Tenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name");

                    b.Property<string>("WebUrl");

                    b.HasKey("Id");
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
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .Annotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.Index("NormalizedName")
                        .Annotation("Relational:Name", "RoleNameIndex");

                    b.Annotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId");

                    b.HasKey("Id");

                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("AllReady.Models.Activity", b =>
                {
                    b.HasOne("AllReady.Models.Campaign")
                        .WithMany()
                        .ForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Location")
                        .WithMany()
                        .ForeignKey("LocationId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("OrganizerId");

                    b.HasOne("AllReady.Models.Tenant")
                        .WithMany()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySignup", b =>
                {
                    b.HasOne("AllReady.Models.Activity")
                        .WithMany()
                        .ForeignKey("ActivityId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySkill", b =>
                {
                    b.HasOne("AllReady.Models.Activity")
                        .WithMany()
                        .ForeignKey("ActivityId");

                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .ForeignKey("SkillId");
                });

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.HasOne("AllReady.Models.Activity")
                        .WithMany()
                        .ForeignKey("ActivityId");

                    b.HasOne("AllReady.Models.Tenant")
                        .WithMany()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.HasOne("AllReady.Models.Tenant")
                        .WithMany()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.HasOne("AllReady.Models.Tenant")
                        .WithMany()
                        .ForeignKey("ManagingTenantId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.CampaignImpact", b =>
                {
                    b.HasOne("AllReady.Models.CampaignImpactType")
                        .WithMany()
                        .ForeignKey("CampaignImpactTypeId");

                    b.HasOne("AllReady.Models.Campaign")
                        .WithOne()
                        .ForeignKey("AllReady.Models.CampaignImpact", "Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.HasOne("AllReady.Models.Campaign")
                        .WithMany()
                        .ForeignKey("CampaignId");

                    b.HasOne("AllReady.Models.Tenant")
                        .WithMany()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.Location", b =>
                {
                    b.HasOne("AllReady.Models.PostalCodeGeo")
                        .WithMany()
                        .ForeignKey("PostalCodePostalCode");
                });

            modelBuilder.Entity("AllReady.Models.Skill", b =>
                {
                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .ForeignKey("ParentSkillId");
                });

            modelBuilder.Entity("AllReady.Models.TaskSignup", b =>
                {
                    b.HasOne("AllReady.Models.AllReadyTask")
                        .WithMany()
                        .ForeignKey("TaskId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("AllReady.Models.TaskSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .ForeignKey("SkillId");

                    b.HasOne("AllReady.Models.AllReadyTask")
                        .WithMany()
                        .ForeignKey("TaskId");
                });

            modelBuilder.Entity("AllReady.Models.UserSkill", b =>
                {
                    b.HasOne("AllReady.Models.Skill")
                        .WithMany()
                        .ForeignKey("SkillId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .ForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .ForeignKey("RoleId");

                    b.HasOne("AllReady.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });
        }
    }
}
