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
        public DbSet<CampaignImpact> CampaignImpacts { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSkill> EventSkills { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PostalCodeGeo> PostalCodes { get; set; }
        public DbSet<AllReadyTask> Tasks { get; set; }
        public DbSet<TaskSkill> TaskSkills { get; set; }
        public DbSet<TaskSignup> TaskSignups { get; set; }
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
        public DbSet<FileAttachment> Attachments { get; set; }
        public DbSet<FileAttachmentContent> AttachmentContents { get; set; }

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
            Map(modelBuilder.Entity<AllReadyTask>());
            Map(modelBuilder.Entity<TaskSkill>());
            Map(modelBuilder.Entity<TaskSignup>());
            Map(modelBuilder.Entity<PostalCodeGeo>());
            Map(modelBuilder.Entity<Skill>());
            Map(modelBuilder.Entity<UserSkill>());
            Map(modelBuilder.Entity<ApplicationUser>());
            Map(modelBuilder.Entity<Organization>());
            Map(modelBuilder.Entity<OrganizationContact>());
            Map(modelBuilder.Entity<CampaignContact>());
            Map(modelBuilder.Entity<Contact>());
            Map(modelBuilder.Entity<CampaignImpact>());
            Map(modelBuilder.Entity<ClosestLocation>());
            Map(modelBuilder.Entity<PostalCodeGeoCoordinate>());
            Map(modelBuilder.Entity<Request>());
            Map(modelBuilder.Entity<Itinerary>());
            Map(modelBuilder.Entity<ItineraryRequest>());
            Map(modelBuilder.Entity<FileAttachment>());
            Map(modelBuilder.Entity<FileAttachmentContent>());
        }

        private void Map(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(x => x.RequestId);
            builder.HasOne(r => r.Organization).WithMany(o => o.Requests).HasForeignKey(r => r.OrganizationId);
            builder.HasOne(r => r.Itinerary);
        }

        private void Map(EntityTypeBuilder<CampaignImpact> builder)
        {
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

        private void Map(EntityTypeBuilder<TaskSignup> builder)
        {
            builder.HasOne(u => u.Task).WithMany(x => x.AssignedVolunteers).HasForeignKey(x => x.TaskId);
        }

        private void Map(EntityTypeBuilder<AllReadyTask> builder)
        {
            builder.HasOne(t => t.Event).WithMany(e => e.Tasks).HasForeignKey(t => t.EventId);
            builder.HasOne(t => t.Organization);
            builder.HasMany(t => t.AssignedVolunteers)
                .WithOne(ts => ts.Task)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(t => t.RequiredSkills).WithOne(ts => ts.Task);
            builder.HasMany(t => t.Attachments);
            builder.Property(p => p.Name).IsRequired();
        }

        private void Map(EntityTypeBuilder<TaskSkill> builder)
        {
            builder.HasKey(acsk => new { acsk.TaskId, acsk.SkillId });
        }

        private void Map(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(a => a.Campaign);
            builder.HasOne(a => a.Location);
            builder.HasMany(a => a.Tasks)
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
            builder.HasOne(c => c.CampaignImpact);
            builder.HasMany(c => c.Events);
            builder.HasOne(t => t.Location);
            builder.HasMany(t => t.CampaignContacts);
            builder.Property(a => a.Name).IsRequired();
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
        }

        public void Map(EntityTypeBuilder<ItineraryRequest> builder)
        {
            builder.HasKey(x => new { x.ItineraryId, x.RequestId });
            builder.HasIndex(x => x.RequestId).IsUnique();
        }

        public void Map(EntityTypeBuilder<FileAttachment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Task);
            builder.HasOne(a => a.Content);
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.MimeType).IsRequired();
        }

        public void Map(EntityTypeBuilder<FileAttachmentContent> builder)
        {
            builder.HasKey(a => a.Id);
        }
    }
}
