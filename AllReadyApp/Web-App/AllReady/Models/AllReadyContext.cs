using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AllReady.Models
{
    public class AllReadyContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Tenant> Tenants { get; set; }

        //public DbSet<UserProfile> UserProfiles { get; set; }

        //public DbSet<UserSecurity> Security { get; set; }

        public DbSet<ActivitySignup> ActivitySignup { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<PostalCodeGeo> PostalCodes { get; set; }

        public DbSet<AllReadyTask> Tasks { get; set; }

        public DbSet<TaskUsers> TaskSignup { get; set; }

        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.ForSqlServer().UseIdentity();

            modelBuilder.Entity<Campaign>()
                .Reference<Tenant>(c => c.ManagingTenant);

            modelBuilder.Entity<Campaign>()
                .Collection<Activity>(c => c.Activities);

            modelBuilder.Entity<CampaignSponsors>()
                .Reference<Campaign>(s => s.Campaign)
                .InverseCollection(c => c.ParticipatingTenants);

            modelBuilder.Entity<CampaignSponsors>()
                .Reference<Tenant>(s => s.Tenant);

            modelBuilder.Entity<Activity>()
                .Reference(a => a.Tenant);

            modelBuilder.Entity<Activity>()
                .Reference(a => a.Campaign);

            modelBuilder.Entity<Activity>()
                .Reference(a => a.Location);

            modelBuilder.Entity<Activity>()
                .Collection<AllReadyTask>(a => a.Tasks);

            modelBuilder.Entity<Activity>()
                .Collection<ActivitySignup>(a => a.UsersSignedUp);

            modelBuilder.Entity<ActivitySignup>()
                .Reference<ApplicationUser>(t => t.User);

            modelBuilder.Entity<ActivitySignup>()
                .Reference<Activity>(t => t.Activity);

            modelBuilder.Entity<AllReadyTask>()
                .Reference(t => t.Activity);

            modelBuilder.Entity<AllReadyTask>()
                .Reference(t => t.Tenant);

            modelBuilder.Entity<AllReadyTask>()
                .Collection(t => t.AssignedVolunteers);

            modelBuilder.Entity<TaskUsers>()
                .Reference(u => u.Task);

            //modelBuilder.Entity<TaskUsers>()
            //	.Reference(u => u.User);

            modelBuilder.Entity<Location>()
                .Reference(l => l.PostalCode);

            modelBuilder.Entity<PostalCodeGeo>()
                .Key(k => k.PostalCode);

            //modelBuilder.Entity<TenantUsers>()
            //	.Reference(u => u.Tenant);

            //modelBuilder.Entity<TenantUsers>()
            //	.Reference(u => u.User);

            //modelBuilder.Entity<UserProfile>()
            //	.Key(k => k.UserId);

            //modelBuilder.Entity<UserSecurity>()
            //	.Key(k => k.UserId);

            modelBuilder.Entity<Resource>()
                .Key(k => k.Id);
        }
    }
}
