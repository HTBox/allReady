using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AllReady.Models
{
    public class AllReadyContext : IdentityDbContext<ApplicationUser>
    {
        public AllReadyContext() { }

        public AllReadyContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Organization> Organizations { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignGoal> CampaignGoals { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSkill> EventSkills { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PostalCodeGeo> PostalCodes { get; set; }
        public DbSet<VolunteerTask> VolunteerTasks { get; set; }
        public DbSet<VolunteerTaskSkill> VolunteerTaskSkills { get; set; }
        public DbSet<VolunteerTaskSignup> VolunteerTaskSignups { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<OrganizationContact> OrganizationContacts { get; set; }
        public DbSet<CampaignContact> CampaignContacts { get; set; }
        public DbSet<ClosestLocation> ClosestLocations { get; set; }
        public DbSet<PostalCodeGeoCoordinate> PostalCodeGeoCoordinates { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<ItineraryRequest> ItineraryRequests { get; set; }
        public DbSet<CampaignManager> CampaignManagers { get; set; }
        public DbSet<EventManager> EventManagers { get; set; }
        public DbSet<EventManagerInvite> EventManagerInvites { get; set; }
        public DbSet<CampaignManagerInvite> CampaignManagerInvites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Keep old database table naming convention
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }

            Map(modelBuilder.Entity<Campaign>());
            Map(modelBuilder.Entity<CampaignSponsors>());
            Map(modelBuilder.Entity<Event>());
            Map(modelBuilder.Entity<EventSkill>());
            Map(modelBuilder.Entity<VolunteerTask>());
            Map(modelBuilder.Entity<VolunteerTaskSkill>());
            Map(modelBuilder.Entity<VolunteerTaskSignup>());
            Map(modelBuilder.Entity<PostalCodeGeo>());
            Map(modelBuilder.Entity<Skill>());
            Map(modelBuilder.Entity<UserSkill>());
            Map(modelBuilder.Entity<ApplicationUser>());
            Map(modelBuilder.Entity<Organization>());
            Map(modelBuilder.Entity<OrganizationContact>());
            Map(modelBuilder.Entity<CampaignContact>());
            Map(modelBuilder.Entity<Contact>());
            Map(modelBuilder.Entity<CampaignGoal>());
            Map(modelBuilder.Entity<ClosestLocation>());
            Map(modelBuilder.Entity<PostalCodeGeoCoordinate>());
            Map(modelBuilder.Entity<Request>());
            Map(modelBuilder.Entity<Itinerary>());
            Map(modelBuilder.Entity<ItineraryRequest>());
            Map(modelBuilder.Entity<CampaignManager>());
            Map(modelBuilder.Entity<EventManager>());
            Map(modelBuilder.Entity<EventManagerInvite>());
            Map(modelBuilder.Entity<CampaignManagerInvite>());
        }

        private void Map(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(x => x.RequestId);
            builder.HasOne(r => r.Organization).WithMany(o => o.Requests).HasForeignKey(r => r.OrganizationId);
            builder.HasOne(r => r.Itinerary);
        }

        private void Map(EntityTypeBuilder<CampaignGoal> builder)
        {
            builder.HasOne(c => c.Campaign);
            builder.Ignore(c => c.PercentComplete);
        }

        private void Map(EntityTypeBuilder<CampaignContact> builder)
        {
            builder.HasKey(tc => new { tc.CampaignId, tc.ContactId, tc.ContactType });
            builder.HasOne(tc => tc.Contact);
            builder.HasOne(tc => tc.Campaign);
        }

        private void Map(EntityTypeBuilder<Contact> builder)
        {
            builder.HasMany(c => c.OrganizationContacts);
            builder.HasMany(c => c.CampaignContacts);
        }

        private void Map(EntityTypeBuilder<OrganizationContact> builder)
        {
            builder.HasKey(tc => new { tc.OrganizationId, tc.ContactId, tc.ContactType });
            builder.HasOne(tc => tc.Contact);
            builder.HasOne(tc => tc.Organization);
        }

        private void Map(EntityTypeBuilder<Organization> builder)
        {
            builder.HasOne(t => t.Location);
            builder.HasMany(t => t.OrganizationContacts);
            builder.Property(t => t.Name).IsRequired();
        }

        private void Map(EntityTypeBuilder<PostalCodeGeo> builder)
        {
            builder.HasKey(k => k.PostalCode);
        }

        private void Map(EntityTypeBuilder<VolunteerTaskSignup> builder)
        {
            builder.HasOne(u => u.VolunteerTask).WithMany(x => x.AssignedVolunteers).HasForeignKey(x => x.VolunteerTaskId);
        }

        private void Map(EntityTypeBuilder<VolunteerTask> builder)
        {
            builder.HasOne(t => t.Event).WithMany(e => e.VolunteerTasks).HasForeignKey(t => t.EventId);
            builder.HasOne(t => t.Organization);
            builder.HasMany(t => t.AssignedVolunteers)
                .WithOne(ts => ts.VolunteerTask)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.RequiredSkills).WithOne(ts => ts.VolunteerTask);
            builder.Property(p => p.Name).IsRequired();
        }

        private void Map(EntityTypeBuilder<VolunteerTaskSkill> builder)
        {
            builder.HasKey(acsk => new { TaskId = acsk.VolunteerTaskId, acsk.SkillId });
        }

        private void Map(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(a => a.Campaign);
            builder.HasOne(a => a.Location);
            builder.HasMany(a => a.VolunteerTasks)
                .WithOne(t => t.Event)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.RequiredSkills).WithOne(acsk => acsk.Event);
            builder.Property(p => p.Name).IsRequired();
            builder.HasMany(x => x.Itineraries).WithOne(x => x.Event).HasForeignKey(x => x.EventId).IsRequired();
            builder.HasMany(x => x.Requests).WithOne(x => x.Event).HasForeignKey(x => x.EventId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        }

        private void Map(EntityTypeBuilder<EventSkill> builder)
        {
            builder.HasKey(acsk => new { acsk.EventId, acsk.SkillId });
        }

        private void Map(EntityTypeBuilder<Skill> builder)
        {
            builder.HasOne(s => s.ParentSkill).WithMany(s => s.ChildSkills).HasForeignKey(s => s.ParentSkillId);
            builder.Ignore(s => s.HierarchicalName);
            builder.Ignore(s => s.DescendantIds);
        }

        private void Map(EntityTypeBuilder<CampaignSponsors> builder)
        {
            builder.HasOne(s => s.Campaign)
                   .WithMany(c => c.ParticipatingOrganizations);
            builder.HasOne(s => s.Organization);
        }
        private void Map(EntityTypeBuilder<Campaign> builder)
        {
            builder.HasOne(c => c.ManagingOrganization);
            builder.HasMany(c => c.CampaignGoals);
            builder.HasMany(c => c.Events);
            builder.HasMany(c => c.Resources).WithOne(c => c.Campaign).HasForeignKey(c => c.CampaignId);
            builder.HasOne(t => t.Location);
            builder.HasMany(t => t.CampaignContacts);
            builder.Property(a => a.Name).IsRequired();
        }

        private void Map(EntityTypeBuilder<Resource> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasOne(r => r.Campaign);
            builder.Property(r => r.Description).IsRequired();
            builder.Property(r => r.Name).IsRequired();
            builder.Property(r => r.ResourceUrl).IsRequired();
        }
        private void Map(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.AssociatedSkills).WithOne(us => us.User);
        }

        private void Map(EntityTypeBuilder<UserSkill> builder)
        {
            builder.HasKey(us => new { us.UserId, us.SkillId });
        }

        private void Map(EntityTypeBuilder<ClosestLocation> builder)
        {
            builder.HasKey(us => new { us.PostalCode });
        }

        private void Map(EntityTypeBuilder<PostalCodeGeoCoordinate> builder)
        {
            builder.HasKey(us => new { us.Latitude, us.Longitude });
        }

        public void Map(EntityTypeBuilder<Itinerary> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Requests).WithOne(x => x.Itinerary).HasForeignKey((x => x.ItineraryId));
            builder.HasMany(x => x.TeamMembers).WithOne(x => x.Itinerary).HasForeignKey(x => x.ItineraryId).IsRequired(false);
            builder.HasOne(x => x.StartLocation);
            builder.HasOne(x => x.EndLocation);

            // Ignore computed properties which are not stored in the database
            builder.Ignore(b => b.StartAddress);
            builder.Ignore(b => b.EndAddress);
            builder.Ignore(b => b.HasAddresses);
        }

        public void Map(EntityTypeBuilder<ItineraryRequest> builder)
        {
            builder.HasKey(x => new { x.ItineraryId, x.RequestId });
            builder.HasIndex(x => x.RequestId).IsUnique();
        }

        private void Map(EntityTypeBuilder<CampaignManager> builder)
        {
            builder.HasKey(x => new { x.UserId, x.CampaignId });

            builder.HasOne(x => x.User)
                .WithMany(u => u.ManagedCampaigns)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Campaign)
                .WithMany(u => u.CampaignManagers)
                .HasForeignKey(x => x.CampaignId);
        }

        private void Map(EntityTypeBuilder<EventManager> builder)
        {
            builder.HasKey(x => new { x.UserId, x.EventId });

            builder.HasOne(x => x.User)
                .WithMany(u => u.ManagedEvents)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Event)
                .WithMany(u => u.EventManagers)
                .HasForeignKey(x => x.EventId);
        }

        private void Map(EntityTypeBuilder<EventManagerInvite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(a => a.InviteeEmailAddress).IsRequired();

            builder.Property(a => a.SenderUserId).IsRequired();

            builder.HasOne(x => x.SenderUser)
                .WithMany(u => u.SentEventManagerInvites)
                .HasForeignKey(x => x.SenderUserId)
                .IsRequired();

            builder.HasOne(x => x.Event)
                .WithMany(u => u.ManagementInvites)
                .HasForeignKey(x => x.EventId)
                .IsRequired();

            builder.Ignore(x => x.IsAccepted);
            builder.Ignore(x => x.IsRejected);
            builder.Ignore(x => x.IsRevoked);
            builder.Ignore(x => x.IsPending);
        }

        private void Map(EntityTypeBuilder<CampaignManagerInvite> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(a => a.InviteeEmailAddress).IsRequired();

            builder.Property(a => a.SenderUserId).IsRequired();

            builder.HasOne(x => x.SenderUser)
                .WithMany(u => u.SentCampaignManagerInvites)
                .HasForeignKey(x => x.SenderUserId)
                .IsRequired();

            builder.HasOne(x => x.Campaign)
                .WithMany(u => u.ManagementInvites)
                .HasForeignKey(x => x.CampaignId)
                .IsRequired();

            builder.Ignore(x => x.IsAccepted);
            builder.Ignore(x => x.IsRejected);
            builder.Ignore(x => x.IsRevoked);
            builder.Ignore(x => x.IsPending);
        }
    }
}
