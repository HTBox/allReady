using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using AllReady.Models;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace AllReady.Migrations
{
    [DbContext(typeof(AllReadyContext))]
    partial class InitialDropBeta7
    {
        public override string Id
        {
            get { return "20150925070212_InitialDropBeta7"; }
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta7-15540")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

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

                    b.Property<string>("OrganizerId");

                    b.Property<DateTime>("StartDateTimeUtc");

                    b.Property<int>("TenantId");

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySignup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<DateTime?>("CheckinDateTime");

                    b.Property<DateTime>("SignupDateTime");

                    b.Property<string>("UserId");

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityId");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDateTimeUtc");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("StartDateTimeUtc");

                    b.Property<int?>("TenantId");

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("AssociatedTenantId");

                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();

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

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .Annotation("MaxLength", 256);

                    b.Key("Id");

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

                    b.Property<string>("ImageUrl");

                    b.Property<int>("ManagingTenantId");

                    b.Property<string>("Name")
                        .Required();

                    b.Property<string>("OrganizerId");

                    b.Property<DateTime>("StartDateTimeUtc");

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CampaignId");

                    b.Property<int?>("TenantId");

                    b.Key("Id");
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

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.PostalCodeGeo", b =>
                {
                    b.Property<string>("PostalCode")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("State");

                    b.Key("PostalCode");
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

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.TaskUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Status");

                    b.Property<DateTime>("StatusDateTimeUtc");

                    b.Property<string>("StatusDescription");

                    b.Property<int?>("TaskId");

                    b.Property<string>("UserId");

                    b.Key("Id");
                });

            modelBuilder.Entity("AllReady.Models.Tenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name");

                    b.Property<string>("WebUrl");

                    b.Key("Id");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();

                    b.Property<string>("Name")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .Annotation("MaxLength", 256);

                    b.Key("Id");

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

                    b.Key("Id");

                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId");

                    b.Key("Id");

                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId");

                    b.Key("LoginProvider", "ProviderKey");

                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Key("UserId", "RoleId");

                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("AllReady.Models.Activity", b =>
                {
                    b.Reference("AllReady.Models.Campaign")
                        .InverseCollection()
                        .ForeignKey("CampaignId");

                    b.Reference("AllReady.Models.Location")
                        .InverseCollection()
                        .ForeignKey("LocationId");

                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("OrganizerId");

                    b.Reference("AllReady.Models.Tenant")
                        .InverseCollection()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.ActivitySignup", b =>
                {
                    b.Reference("AllReady.Models.Activity")
                        .InverseCollection()
                        .ForeignKey("ActivityId");

                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("AllReady.Models.AllReadyTask", b =>
                {
                    b.Reference("AllReady.Models.Activity")
                        .InverseCollection()
                        .ForeignKey("ActivityId");

                    b.Reference("AllReady.Models.Tenant")
                        .InverseCollection()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.ApplicationUser", b =>
                {
                    b.Reference("AllReady.Models.Tenant")
                        .InverseCollection()
                        .ForeignKey("AssociatedTenantId");
                });

            modelBuilder.Entity("AllReady.Models.Campaign", b =>
                {
                    b.Reference("AllReady.Models.Tenant")
                        .InverseCollection()
                        .ForeignKey("ManagingTenantId");

                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("OrganizerId");
                });

            modelBuilder.Entity("AllReady.Models.CampaignSponsors", b =>
                {
                    b.Reference("AllReady.Models.Campaign")
                        .InverseCollection()
                        .ForeignKey("CampaignId");

                    b.Reference("AllReady.Models.Tenant")
                        .InverseCollection()
                        .ForeignKey("TenantId");
                });

            modelBuilder.Entity("AllReady.Models.Location", b =>
                {
                    b.Reference("AllReady.Models.PostalCodeGeo")
                        .InverseCollection()
                        .ForeignKey("PostalCodePostalCode");
                });

            modelBuilder.Entity("AllReady.Models.TaskUsers", b =>
                {
                    b.Reference("AllReady.Models.AllReadyTask")
                        .InverseCollection()
                        .ForeignKey("TaskId");

                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");

                    b.Reference("AllReady.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
        }
    }
}
