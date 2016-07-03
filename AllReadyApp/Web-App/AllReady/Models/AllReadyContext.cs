using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllReady.Models
{
  public class AllReadyContext : IdentityDbContext<ApplicationUser>
  {
    public AllReadyContext()
    {

    }

    public AllReadyContext(DbContextOptions options) : base(options)
    {

    }

    public virtual DbSet<Organization> Organizations { get; set; }
    public DbSet<EventSignup> EventSignup { get; set; }
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      Map(modelBuilder.Entity<Campaign>());
      Map(modelBuilder.Entity<CampaignSponsors>());
      Map(modelBuilder.Entity<Event>());
      Map(modelBuilder.Entity<EventSkill>());
      Map(modelBuilder.Entity<EventSignup>());
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
    }

    private void Map(EntityTypeBuilder<CampaignImpact> builder)
    {
      builder.ToTable("CampaignImpact");
      builder.Ignore(c => c.PercentComplete);
    }

    private void Map(EntityTypeBuilder<CampaignContact> builder)
    {
      builder.ToTable("CampaignContact");
      builder.HasKey(tc => new { tc.CampaignId, tc.ContactId, tc.ContactType });
      builder.HasOne(tc => tc.Contact);
      builder.HasOne(tc => tc.Campaign);
    }

    private void Map(EntityTypeBuilder<Contact> builder)
    {
      builder.ToTable("Contact");
      builder.HasMany(c => c.OrganizationContacts);
      builder.HasMany(c => c.CampaignContacts);
    }

    private void Map(EntityTypeBuilder<OrganizationContact> builder)
    {
      builder.ToTable("OrganizationContact");
      builder.HasKey(tc => new { tc.OrganizationId, tc.ContactId, tc.ContactType });
      builder.HasOne(tc => tc.Contact);
      builder.HasOne(tc => tc.Organization);
    }

    private void Map(EntityTypeBuilder<Organization> builder)
    {
      builder.ToTable("Organization");
      builder.HasOne(t => t.Location);
      builder.HasMany(t => t.OrganizationContacts);
      builder.Property(t => t.Name).IsRequired();
    }

    private void Map(EntityTypeBuilder<PostalCodeGeo> builder)
    {
      builder.ToTable("PostalCodeGeo");
      builder.HasKey(k => k.PostalCode);
    }

    private void Map(EntityTypeBuilder<TaskSignup> builder)
    {
      builder.ToTable("TaskSignup");
      builder.HasOne(u => u.Task);
    }

    private void Map(EntityTypeBuilder<AllReadyTask> builder)
    {
      builder.ToTable("AllReadyTask");
      builder.HasOne(t => t.Event);
      builder.HasOne(t => t.Organization);
      builder.HasMany(t => t.AssignedVolunteers);
      builder.HasMany(t => t.RequiredSkills).WithOne(ts => ts.Task);
      builder.Property(p => p.Name).IsRequired();
    }

    private void Map(EntityTypeBuilder<TaskSkill> builder)
    {
      builder.ToTable("TaskSkill");
      builder.HasKey(acsk => new { acsk.TaskId, acsk.SkillId });
    }

    private void Map(EntityTypeBuilder<EventSignup> builder)
    {
      builder.ToTable("EventSignup");
      builder.HasOne(t => t.User);
      builder.HasOne(t => t.Event);
      builder.Property(t => t.SignupDateTime)
             .IsRequired();
    }

    private void Map(EntityTypeBuilder<Event> builder)
    {
      builder.ToTable("Event");
      builder.HasOne(a => a.Campaign);
      builder.HasOne(a => a.Location);
      builder.HasMany(a => a.Tasks);
      builder.HasMany(a => a.UsersSignedUp);
      builder.HasMany(a => a.RequiredSkills).WithOne(acsk => acsk.Event);
      builder.Property(p => p.Name).IsRequired();
    }

    private void Map(EntityTypeBuilder<EventSkill> builder)
    {
      builder.ToTable("EventSkill");
      builder.HasKey(acsk => new { acsk.EventId, acsk.SkillId });
    }

    private void Map(EntityTypeBuilder<Skill> builder)
    {
      builder.ToTable("Skill");
      builder.HasOne(s => s.ParentSkill);
      builder.Ignore(s => s.HierarchicalName);
    }

    private void Map(EntityTypeBuilder<CampaignSponsors> builder)
    {
      builder.ToTable("CampaignContact");
      builder.HasOne(s => s.Campaign)
             .WithMany(c => c.ParticipatingOrganizations);
      builder.HasOne(s => s.Organization);
    }

    private void Map(EntityTypeBuilder<Campaign> builder)
    {
      builder.ToTable("Campaign");
      builder.HasOne(c => c.ManagingOrganization);
      builder.HasOne(c => c.CampaignImpact);
      builder.HasMany(c => c.Events);
      builder.HasOne(t => t.Location);
      builder.HasMany(t => t.CampaignContacts);
      builder.Property(a => a.Name).IsRequired();
    }

    private void Map(EntityTypeBuilder<ApplicationUser> builder)
    {
      builder.ToTable("ApplicationUser");
      builder.HasMany(u => u.AssociatedSkills).WithOne(us => us.User);
    }

    private void Map(EntityTypeBuilder<UserSkill> builder)
    {
      builder.ToTable("UserSkill");
      builder.HasKey(us => new { us.UserId, us.SkillId });
    }

    private void Map(EntityTypeBuilder<ClosestLocation> builder)
    {
      builder.ToTable("ClosestLocation");
      builder.HasKey(us => new { us.PostalCode });
    }

    private void Map(EntityTypeBuilder<PostalCodeGeoCoordinate> builder)
    {
      builder.ToTable("PostalCodeGeoCoordinate");
      builder.HasKey(us => new { us.Latitude, us.Longitude });
    }
  }
}
