using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Framework.Configuration;

namespace AllReady.Models
{
  public class AllReadyContext : IdentityDbContext<ApplicationUser>
  {
    private IConfiguration _configuration;

    public AllReadyContext(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (_configuration["Data:DefaultConnection:UseInMemory"].ToLowerInvariant() == "true")
      {
        optionsBuilder.UseInMemoryDatabase();
      }
      else
      {
        optionsBuilder.UseSqlServer(_configuration["Data:DefaultConnection:AzureConnectionString"]);
      }
    }

    public DbSet<Tenant> Tenants { get; set; }

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

      modelBuilder.UseSqlServerIdentityColumns();

      modelBuilder.Entity<Campaign>()
          .HasOne(c => c.ManagingTenant);

      modelBuilder.Entity<Campaign>()
          .HasMany(c => c.Activities);

      modelBuilder.Entity<CampaignSponsors>()
          .HasOne(s => s.Campaign)
          .WithMany(c => c.ParticipatingTenants);

      modelBuilder.Entity<CampaignSponsors>()
          .HasOne(s => s.Tenant);

      modelBuilder.Entity<Activity>()
          .HasOne(a => a.Tenant);

      modelBuilder.Entity<Activity>()
          .HasOne(a => a.Campaign);

      modelBuilder.Entity<Activity>()
          .HasOne(a => a.Location);

      modelBuilder.Entity<Activity>()
          .HasMany(a => a.Tasks);

      modelBuilder.Entity<Activity>()
          .HasMany(a => a.UsersSignedUp);

      modelBuilder.Entity<ActivitySignup>()
          .HasOne(t => t.User);

      modelBuilder.Entity<ActivitySignup>()
          .HasOne(t => t.Activity);

      modelBuilder.Entity<AllReadyTask>()
          .HasOne(t => t.Activity);

      modelBuilder.Entity<AllReadyTask>()
          .HasOne(t => t.Tenant);

      modelBuilder.Entity<AllReadyTask>()
          .HasMany(t => t.AssignedVolunteers);

      modelBuilder.Entity<TaskUsers>()
          .HasOne(u => u.Task);

      modelBuilder.Entity<Location>()
          .HasOne(l => l.PostalCode);

      modelBuilder.Entity<PostalCodeGeo>()
          .HasKey(k => k.PostalCode);

      modelBuilder.Entity<Resource>()
          .HasKey(k => k.Id);
    }

  }
}
