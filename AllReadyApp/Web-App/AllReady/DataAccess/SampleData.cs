using Microsoft.AspNet.Identity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

using AllReady.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using AllReady.DataAccess;
using Microsoft.Dnx.Runtime;

namespace AllReady.Models
{
  public static class SampleData
  {
    private static ITaskIdProvider _taskIdProvider = new TaskIdProvider();
    public static void InsertTestData(AllReadyContext dbContext)
    {
      _taskIdProvider.Reset();
      // Avoid polluting the database if there's already something in there.
      if (dbContext.Locations.Any() ||
          dbContext.Tenants.Any() ||
          dbContext.Tasks.Any() ||
          dbContext.Campaigns.Any() ||
          dbContext.Activities.Any() ||
          dbContext.Resources.Any())
      {
        return;
      }

      List<Tenant> tenants = new List<Tenant>();
      dbContext.PostalCodes.AddRange(GetPostalCodes(dbContext));
      dbContext.SaveChanges();
      List<Location> locations = GetLocations(dbContext);
      List<TaskUsers> users = new List<TaskUsers>();
      List<Activity> activities = new List<Activity>();
      List<Campaign> campaigns = new List<Campaign>();
      List<AllReadyTask> tasks = new List<AllReadyTask>();
      List<Resource> resources = new List<Resource>();

      #region Tenant
      Tenant htb = new Tenant()
      {
        Name = "Humanitarian Toolbox",
        LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
        WebUrl = "http://www.htbox.org",
        Campaigns = new List<Campaign>()
      };
      #endregion
      #region Campaign

      Campaign firePrev = new Campaign()
      {
        Name = "Neighborhood Fire Prevention Days",
        ManagingTenant = htb
      };
      htb.Campaigns.Add(firePrev);
      Campaign smokeDet = new Campaign()
      {
        Name = "Working Smoke Detectors Save Lives",
        ManagingTenant = htb,
        StartDateTimeUtc = DateTime.Today.AddMonths(-1).ToUniversalTime(),
        EndDateTimeUtc = DateTime.Today.AddMonths(1).ToUniversalTime()
      };
      htb.Campaigns.Add(smokeDet);
      Campaign financial = new Campaign()
      {
        Name = "Everyday Financial Safety",
        ManagingTenant = htb
      };
      htb.Campaigns.Add(financial);
      Campaign safetyKit = new Campaign()
      {
        Name = "Simple Safety Kit Building",
        ManagingTenant = htb
      };
      htb.Campaigns.Add(safetyKit);
      Campaign carSafe = new Campaign()
      {
        Name = "Family Safety In the Car",
        ManagingTenant = htb
      };
      htb.Campaigns.Add(carSafe);
      Campaign escapePlan = new Campaign()
      {
        Name = "Be Ready to Get Out: Have a Home Escape Plan",
        ManagingTenant = htb
      };
      htb.Campaigns.Add(escapePlan);
      #endregion
      #region Activity
      Activity queenAnne = new Activity()
      {
        Name = "Queen Anne Fire Prevention Day",
        StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 4, 15, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      queenAnne.Tasks = GetSomeTasks(queenAnne, htb);
      tasks.AddRange(queenAnne.Tasks);
      Activity ballard = new Activity()
      {
        Name = "Ballard Fire Prevention Day",
        StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 4, 14, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb

      };
      ballard.Tasks = GetSomeTasks(ballard, htb);
      tasks.AddRange(ballard.Tasks);
      Activity madrona = new Activity()
      {
        Name = "Madrona Fire Prevention Day",
        StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 4, 14, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      madrona.Tasks = GetSomeTasks(madrona, htb);
      tasks.AddRange(madrona.Tasks);
      Activity southLoopSmoke = new Activity()
      {
        Name = "Smoke Detector Installation and Testing-South Loop",
        StartDateTimeUtc = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 17, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      southLoopSmoke.Tasks = GetSomeTasks(southLoopSmoke, htb);
      tasks.AddRange(southLoopSmoke.Tasks);
      Activity northLoopSmoke = new Activity()
      {
        Name = "Smoke Detector Installation and Testing-Near North Side",
        StartDateTimeUtc = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 17, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      northLoopSmoke.Tasks = GetSomeTasks(northLoopSmoke, htb);
      tasks.AddRange(northLoopSmoke.Tasks);
      Activity rentersInsurance = new Activity()
      {
        Name = "Renters Insurance Education Door to Door and a bag of chips",
        Description = "description for the win",
        StartDateTimeUtc = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 17, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      rentersInsurance.Tasks = GetSomeTasks(rentersInsurance, htb);
      tasks.AddRange(rentersInsurance.Tasks);
      Activity rentersInsuranceEd = new Activity()
      {
        Name = "Renters Insurance Education Door to Door (woop woop)",
        Description = "another great description",
        StartDateTimeUtc = new DateTime(2015, 7, 12, 8, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 12, 17, 0, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      rentersInsuranceEd.Tasks = GetSomeTasks(rentersInsuranceEd, htb);
      tasks.AddRange(rentersInsuranceEd.Tasks);
      Activity safetyKitBuild = new Activity()
      {
        Name = "Safety Kit Assembly Volunteer Day",
        Description = "Full day of volunteers building kits",
        StartDateTimeUtc = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 16, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      safetyKitBuild.Tasks = GetSomeTasks(safetyKitBuild, htb);
      tasks.AddRange(safetyKitBuild.Tasks);

      Activity safetyKitHandout = new Activity()
      {
        Name = "Safety Kit Distribution Weekend",
        Description = "Handing out kits at local fire stations",
        StartDateTimeUtc = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 16, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      safetyKitHandout.Tasks = GetSomeTasks(safetyKitHandout, htb);
      tasks.AddRange(safetyKitHandout.Tasks);
      Activity carSeatTest1 = new Activity()
      {
        Name = "Car Seat Testing-Naperville",
        Description = "Checking car seats at local fire stations after last day of school year",
        StartDateTimeUtc = new DateTime(2015, 7, 10, 9, 30, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 10, 15, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      carSeatTest1.Tasks = GetSomeTasks(carSeatTest1, htb);
      tasks.AddRange(carSeatTest1.Tasks);
      Activity carSeatTest2 = new Activity()
      {
        Name = "Car Seat and Tire Pressure Checking Volunteer Day",
        Description = "Checking those things all day at downtown train station parking",
        StartDateTimeUtc = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 19, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      carSeatTest2.Tasks = GetSomeTasks(carSeatTest2, htb);
      tasks.AddRange(carSeatTest2.Tasks);
      Activity homeFestival = new Activity()
      {
        Name = "Park District Home Safety Festival",
        Description = "At downtown park district(adjacent to pool)",
        StartDateTimeUtc = new DateTime(2015, 7, 11, 12, 0, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 11, 16, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      homeFestival.Tasks = GetSomeTasks(homeFestival, htb);
      tasks.AddRange(homeFestival.Tasks);
      Activity homeEscape = new Activity()
      {
        Name = "Home Escape Plan Flyer Distribution",
        Description = "Handing out flyers door to door in several areas of town after school/ work hours.Streets / blocks will vary but number of volunteers.",
        StartDateTimeUtc = new DateTime(2015, 7, 15, 15, 30, 0).ToUniversalTime(),
        EndDateTimeUtc = new DateTime(2015, 7, 15, 20, 30, 0).ToUniversalTime(),
        Location = GetRandom<Location>(locations),
        Tenant = htb
      };
      homeEscape.Tasks = GetSomeTasks(homeEscape, htb);
      tasks.AddRange(homeEscape.Tasks);
      #endregion
      #region Link campaign and activity
      firePrev.Activities = new List<Activity>();
      firePrev.Activities.Add(queenAnne);
      firePrev.Activities.Add(ballard);
      firePrev.Activities.Add(madrona);
      smokeDet.Activities = new List<Activity>();
      smokeDet.Activities.Add(southLoopSmoke);
      smokeDet.Activities.Add(northLoopSmoke);
      financial.Activities = new List<Activity>();
      financial.Activities.Add(rentersInsurance);
      financial.Activities.Add(rentersInsuranceEd);
      safetyKit.Activities = new List<Activity>();
      safetyKit.Activities.Add(safetyKitBuild);
      safetyKit.Activities.Add(safetyKitHandout);
      carSafe.Activities = new List<Activity>();
      carSafe.Activities.Add(carSeatTest1);
      carSafe.Activities.Add(carSeatTest2);
      escapePlan.Activities = new List<Activity>();
      escapePlan.Activities.Add(homeFestival);
      escapePlan.Activities.Add(homeEscape);
      #endregion
      #region Add Campaigns and Activities
      tenants.Add(htb);
      campaigns.Add(firePrev);
      campaigns.Add(smokeDet);
      campaigns.Add(financial);
      campaigns.Add(escapePlan);
      campaigns.Add(safetyKit);
      campaigns.Add(carSafe);

      activities.AddRange(firePrev.Activities);
      activities.AddRange(smokeDet.Activities);
      activities.AddRange(financial.Activities);
      activities.AddRange(escapePlan.Activities);
      activities.AddRange(safetyKit.Activities);
      activities.AddRange(carSafe.Activities);
      #endregion

      #region Insert Resource items into Resources
      resources.Add(new Resource
      {
        Id = 1,
        Name = "allReady Partner Name",
        Description = "allready Partner Description",
        PublishDateBegin = DateTime.Today,
        PublishDateEnd = DateTime.Today.AddDays(14),
        MediaUrl = "",
        ResourceUrl = "",
        CategoryTag = "Partners"
      });
      resources.Add(new Resource
      {
        Id = 2,
        Name = "allReady Partner Name 2",
        Description = "allready Partner Description 2",
        PublishDateBegin = DateTime.Today.AddDays(-3),
        PublishDateEnd = DateTime.Today.AddDays(-1),
        MediaUrl = "",
        ResourceUrl = "",
        CategoryTag = "Partners"
      });
      #endregion

      #region Insert into DB
      dbContext.Locations.AddRange(locations);
      dbContext.Tenants.AddRange(tenants);
      dbContext.Tasks.AddRange(tasks);
      dbContext.Campaigns.AddRange(campaigns);
      dbContext.Activities.AddRange(activities);
      dbContext.Resources.AddRange(resources);
      dbContext.SaveChanges();
      #endregion
    }
    #region Sample Data Helper methods
    private static T GetRandom<T>(List<T> list)
    {
      Random rand = new Random();
      return list[rand.Next(list.Count)];
    }

    private static List<AllReadyTask> GetSomeTasks(Activity activity, Tenant tenant)
    {
      List<AllReadyTask> value = new List<AllReadyTask>();
      for (int i = 0; i < 5; i++)
      {
        var tempId = _taskIdProvider.NextValue();
        value.Add(new AllReadyTask()
        {
          Id = tempId,
          Activity = activity,
          Description = "Description of a very important task # " + i,
          Name = "Task # " + tempId,
          EndDateTimeUtc = DateTime.Now.AddDays(i),
          StartDateTimeUtc = DateTime.Now.AddDays(i - 1),
          Tenant = tenant
        });
      }
      return value;
    }

    private static Location CreateLocation(string address1, string city, string state, string postalCode, AllReadyContext ctx)
    {
      Location ret = new Location();
      ret.Address1 = address1;
      ret.City = city;
      ret.State = state;
      ret.Country = "US";
      ret.PostalCode = ctx.PostalCodes.FirstOrDefault(p => p.PostalCode == postalCode);
      ret.Name = "Humanitarian Toolbox location";
      ret.PhoneNumber = "1-425-555-1212";
      return ret;
    }

    private static List<PostalCodeGeo> GetPostalCodes(AllReadyContext ctx)
    {
      var postalCodes = new List<PostalCodeGeo>();
      postalCodes.Add(new PostalCodeGeo() { City = "Remond", State = "WA", PostalCode = "98052" });
      postalCodes.Add(new PostalCodeGeo() { City = "Bellevue", State = "WA", PostalCode = "98004" });
      postalCodes.Add(new PostalCodeGeo() { City = "Seattle", State = "WA", PostalCode = "98116" });
      postalCodes.Add(new PostalCodeGeo() { City = "Seattle", State = "WA", PostalCode = "98117" });
      postalCodes.Add(new PostalCodeGeo() { City = "Bellevue", State = "WA", PostalCode = "98007" });
      postalCodes.Add(new PostalCodeGeo() { City = "Issaquah", State = "WA", PostalCode = "98027" });
      postalCodes.Add(new PostalCodeGeo() { City = "Kirkland", State = "WA", PostalCode = "98034" });
      postalCodes.Add(new PostalCodeGeo() { City = "Kirkland", State = "WA", PostalCode = "98033" });
      return postalCodes;
    }

    private static List<Location> GetLocations(AllReadyContext ctx)
    {
      var geoService = new GeoService();

      var ret = new List<Location>()
            {
                CreateLocation("1 Microsoft Way", "Redmond", "WA", "98052", ctx),
                CreateLocation("15563 Ne 31st St", "Redmond", "WA", "98052", ctx),
                CreateLocation("700 Bellevue Way Ne", "Bellevue", "WA", "98004", ctx),
                CreateLocation("1702 Alki Ave SW", "Seattle", "WA", "98116", ctx),
                CreateLocation("8498 Seaview Pl NW", "Seattle", "WA", "98117", ctx),
                CreateLocation("6046 W Lake Sammamish Pkwy Ne", "Redmond", "WA", "98052", ctx),
                CreateLocation("7031 148th Ave Ne", "Redmond", "WA", "98052", ctx),
                CreateLocation("2430 148th Ave SE", "Bellevue", "WA", "98007", ctx),
                CreateLocation("2000 NW Sammamish Rd", "Issaquah", "WA", "98027", ctx),
                CreateLocation("9703 Ne Juanita Dr", "Kirkland", "WA", "98034", ctx),
                CreateLocation("25 Lakeshore Plaza Dr", "Kirkland", "Washington", "98033", ctx),
                CreateLocation("633 Waverly Way", "Kirkland", "WA", "98033", ctx),
            };

      return ret;
    }
    #endregion

    /// <summary>
    /// Creates a administrative user who can manage the inventory.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task CreateAdminUser(IServiceProvider serviceProvider, AllReadyContext dbContext)
    {
      var appEnv = serviceProvider.GetService<IApplicationEnvironment>();

      var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                  .AddJsonFile("config.json")
                  .AddUserSecrets()
                  .AddEnvironmentVariables();

      var configuration = builder.Build();

      var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

      var user = await userManager.FindByNameAsync(configuration["DefaultAdminUsername"]);
      if (user == null)
      {
        user = new ApplicationUser { UserName = configuration["DefaultAdminUsername"], Email = configuration["DefaultAdminUsername"] };
        user.EmailConfirmed = true;
        await userManager.CreateAsync(user, configuration["DefaultAdminPassword"]);
        await userManager.AddClaimAsync(user, new Claim("UserType", "SiteAdmin"));

        user = new ApplicationUser { UserName = configuration["DefaultTenantUsername"], Email = configuration["DefaultTenantUsername"] };
        // For the sake of being able to exercise Tenant-specific stuff, we need to associate a tenant.
        user.EmailConfirmed = true;
        user.AssociatedTenant = dbContext.Tenants.First();
        await userManager.CreateAsync(user, configuration["DefaultAdminPassword"]);
        await userManager.AddClaimAsync(user, new Claim("UserType", "TenantAdmin"));

        user = new ApplicationUser { UserName = configuration["DefaultUsername"], Email = configuration["DefaultUsername"] };
        user.EmailConfirmed = true;
        await userManager.CreateAsync(user, configuration["DefaultAdminPassword"]);
      }
    }
  }
}
