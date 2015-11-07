using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public interface IAllReadyContext
    {
        DbSet<Tenant> Tenants { get; set; }
        DbSet<ActivitySignup> ActivitySignup { get; set; }
        DbSet<Campaign> Campaigns { get; set; }
        DbSet<CampaignImpact> CampaignImpacts { get; set; }
        DbSet<CampaignImpactType> CampaignImpactTypes { get; set; }
        DbSet<Activity> Activities { get; set; }
        DbSet<ActivitySkill> ActivitySkills { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<PostalCodeGeo> PostalCodes { get; set; }
        DbSet<AllReadyTask> Tasks { get; set; }
        DbSet<TaskSkill> TaskSkills { get; set; }
        DbSet<TaskSignup> TaskSignups { get; set; }
        DbSet<Resource> Resources { get; set; }
        DbSet<Skill> Skills { get; set; }
        DbSet<UserSkill> UserSkills { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
        DbSet<IdentityUserLogin<string>> UserLogins { get; set; }
        DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<IdentityRoleClaim<string>> RoleClaims { get; set; }
        void SaveChanges();
        void Update(object entity);
    }
}