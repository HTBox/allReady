using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Models
{
    public class SampleDataGenerator
    {
        private readonly AllReadyContext _context;
        private readonly SampleDataSettings _settings;
        private readonly GeneralSettings _generalSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public SampleDataGenerator(AllReadyContext context, IOptions<SampleDataSettings> options, IOptions<GeneralSettings> generalSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _settings = options.Value;
            _generalSettings = generalSettings.Value;
            _userManager = userManager;
        }

        //private static ITaskIdProvider _taskIdProvider = new TaskIdProvider();
        public void InsertTestData()
        {
            // Avoid polluting the database if there's already something in there.
            if (_context.Locations.Any() ||
                _context.Organizations.Any() ||
                _context.Tasks.Any() ||
                _context.Campaigns.Any() ||
                _context.Activities.Any() ||
                _context.ActivitySkills.Any() ||
                _context.Skills.Any() ||
                _context.Resources.Any())
            {
                return;
            }
            // new up some data
            List<Organization> tenants = new List<Organization>();

            #region postalCodes
            var existingPostalCode = _context.PostalCodes.ToList();
            _context.PostalCodes.AddRange(GetPostalCodes(existingPostalCode));
            #endregion

            List<Tenant> tenants = new List<Tenant>();
            List<Skill> tenantSkills = new List<Skill>();
            List<Location> locations = GetLocations();
            List<ApplicationUser> users = new List<ApplicationUser>();
            List<TaskSignup> taskSignups = new List<TaskSignup>();
            List<Activity> activities = new List<Activity>();
            List<ActivitySkill> activitySkills = new List<ActivitySkill>();
            List<Campaign> campaigns = new List<Campaign>();
            List<AllReadyTask> tasks = new List<AllReadyTask>();
            List<Resource> resources = new List<Resource>();
            List<ActivitySignup> activitySignups = new List<ActivitySignup>();
            List<Contact> contacts = GetContacts();
            var skills = new List<Skill>();

            #region Skills
            var medical = new Skill() { Name = "Medical", Description = "specific enough, right?" };
            var cprCertified = new Skill() { Name = "CPR Certified", ParentSkill = medical, Description = "ha ha ha ha, stayin alive" };
            var md = new Skill() { Name = "MD", ParentSkill = medical, Description = "Trust me, I'm a doctor" };
            var surgeon = new Skill() { Name = "Surgeon", ParentSkill = md, Description = "cut open; sew shut; play 18 holes" };
            skills.AddRange(new[] { medical, cprCertified, md, surgeon });
            #endregion

            #region Tenant
            Organization htb = new Organization()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Location = locations.FirstOrDefault(),
                Campaigns = new List<Campaign>(), 
                OrganizationContacts = new List<OrganizationContact>(),
                
            };
            #endregion
            
            #region Tenant Skills
            tenantSkills.Add(new Skill()
            {
                Name = "Code Ninja",
                Description = "Ability to commit flawless code without review or testing",
                OwningOrganization = htb
            });
            #endregion

            #region Campaign

            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            var smokeDetImpact = new CampaignImpact
            {
                ImpactType = ImpactType.Numeric,
                NumericImpactGoal = 10000,
                CurrentImpactLevel = 6722,
                Display = true,
                TextualImpactGoal = "Total number of smoke detectors installed."
            };
            _context.CampaignImpacts.Add(smokeDetImpact);
            Campaign smokeDet = new Campaign()
            {
                Name = "Working Smoke Detectors Save Lives",
                ManagingOrganization = htb,
                StartDateTime = DateTime.Today.AddMonths(-1).ToUniversalTime(),
                EndDateTime = DateTime.Today.AddMonths(1).ToUniversalTime(),
                CampaignImpact = smokeDetImpact,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(smokeDet);
            Campaign financial = new Campaign()
            {
                Name = "Everyday Financial Safety",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(financial);
            Campaign safetyKit = new Campaign()
            {
                Name = "Simple Safety Kit Building",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(safetyKit);
            Campaign carSafe = new Campaign()
            {
                Name = "Family Safety In the Car",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(carSafe);
            Campaign escapePlan = new Campaign()
            {
                Name = "Be Ready to Get Out: Have a Home Escape Plan",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(escapePlan);
            #endregion

            #region Activity
            Activity queenAnne = new Activity()
            {
                Name = "Queen Anne Fire Prevention Day",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                RequiredSkills = new List<ActivitySkill>()
            };
            queenAnne.Tasks = GetSomeTasks(queenAnne, htb);
            var ask = new ActivitySkill() { Skill = surgeon, Activity = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            activitySkills.Add(ask);
            ask = new ActivitySkill() { Skill = cprCertified, Activity = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            activitySkills.Add(ask);
            tasks.AddRange(queenAnne.Tasks);

            Activity ballard = new Activity()
            {
                Name = "Ballard Fire Prevention Day",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 14, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = firePrev
            };
            ballard.Tasks = GetSomeTasks(ballard, htb);
            tasks.AddRange(ballard.Tasks);
            Activity madrona = new Activity()
            {
                Name = "Madrona Fire Prevention Day",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 14, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = firePrev
            };
            madrona.Tasks = GetSomeTasks(madrona, htb);
            tasks.AddRange(madrona.Tasks);
            Activity southLoopSmoke = new Activity()
            {
                Name = "Smoke Detector Installation and Testing-South Loop",
                StartDateTime = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 17, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = smokeDet
            };
            southLoopSmoke.Tasks = GetSomeTasks(southLoopSmoke, htb);
            tasks.AddRange(southLoopSmoke.Tasks);
            Activity northLoopSmoke = new Activity()
            {
                Name = "Smoke Detector Installation and Testing-Near North Side",
                StartDateTime = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 17, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = smokeDet
            };
            northLoopSmoke.Tasks = GetSomeTasks(northLoopSmoke, htb);
            tasks.AddRange(northLoopSmoke.Tasks);
            Activity rentersInsurance = new Activity()
            {
                Name = "Renters Insurance Education Door to Door and a bag of chips",
                Description = "description for the win",
                StartDateTime = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 7, 11, 17, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = financial
            };
            rentersInsurance.Tasks = GetSomeTasks(rentersInsurance, htb);
            tasks.AddRange(rentersInsurance.Tasks);
            Activity rentersInsuranceEd = new Activity()
            {
                Name = "Renters Insurance Education Door to Door (woop woop)",
                Description = "another great description",
                StartDateTime = new DateTime(2015, 7, 12, 8, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 12, 17, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = financial
            };
            rentersInsuranceEd.Tasks = GetSomeTasks(rentersInsuranceEd, htb);
            tasks.AddRange(rentersInsuranceEd.Tasks);
            Activity safetyKitBuild = new Activity()
            {
                Name = "Safety Kit Assembly Volunteer Day",
                Description = "Full day of volunteers building kits",
                StartDateTime = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = safetyKit
            };
            safetyKitBuild.Tasks = GetSomeTasks(safetyKitBuild, htb);
            tasks.AddRange(safetyKitBuild.Tasks);

            Activity safetyKitHandout = new Activity()
            {
                Name = "Safety Kit Distribution Weekend",
                Description = "Handing out kits at local fire stations",
                StartDateTime = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = safetyKit
            };
            safetyKitHandout.Tasks = GetSomeTasks(safetyKitHandout, htb);
            tasks.AddRange(safetyKitHandout.Tasks);
            Activity carSeatTest1 = new Activity()
            {
                Name = "Car Seat Testing-Naperville",
                Description = "Checking car seats at local fire stations after last day of school year",
                StartDateTime = new DateTime(2015, 7, 10, 9, 30, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 10, 15, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = carSafe
            };
            carSeatTest1.Tasks = GetSomeTasks(carSeatTest1, htb);
            tasks.AddRange(carSeatTest1.Tasks);
            Activity carSeatTest2 = new Activity()
            {
                Name = "Car Seat and Tire Pressure Checking Volunteer Day",
                Description = "Checking those things all day at downtown train station parking",
                StartDateTime = new DateTime(2015, 7, 11, 8, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 11, 19, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = carSafe
            };
            carSeatTest2.Tasks = GetSomeTasks(carSeatTest2, htb);
            tasks.AddRange(carSeatTest2.Tasks);
            Activity homeFestival = new Activity()
            {
                Name = "Park District Home Safety Festival",
                Description = "At downtown park district(adjacent to pool)",
                StartDateTime = new DateTime(2015, 7, 11, 12, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = safetyKit
            };
            homeFestival.Tasks = GetSomeTasks(homeFestival, htb);
            tasks.AddRange(homeFestival.Tasks);
            Activity homeEscape = new Activity()
            {
                Name = "Home Escape Plan Flyer Distribution",
                Description = "Handing out flyers door to door in several areas of town after school/ work hours.Streets / blocks will vary but number of volunteers.",
                StartDateTime = new DateTime(2015, 7, 15, 15, 30, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 15, 20, 30, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Campaign = escapePlan
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
            _context.Skills.AddRange(skills);
            _context.Contacts.AddRange(contacts);
            _context.ActivitySkills.AddRange(activitySkills);
            _context.Locations.AddRange(locations);
            _context.Organizations.AddRange(tenants);
            _context.Tasks.AddRange(tasks);
            _context.Campaigns.AddRange(campaigns);
            _context.Activities.AddRange(activities);
            _context.Resources.AddRange(resources);
            //_context.SaveChanges();
            #endregion

            #region Users for Activities
            var username1 = $"{_settings.DefaultUsername}1.com";
            var username2 = $"{_settings.DefaultUsername}2.com";
            var username3 = $"{_settings.DefaultUsername}3.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone };
            _userManager.CreateAsync(user1, _settings.DefaultAdminPassword).Wait();
            users.Add(user1);
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone };
            _userManager.CreateAsync(user2, _settings.DefaultAdminPassword).Wait();
            users.Add(user2);
            var user3 = new ApplicationUser { UserName = username3, Email = username3, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone };
            _userManager.CreateAsync(user3, _settings.DefaultAdminPassword).Wait();
            users.Add(user3);
            #endregion

            #region ActvitySignups
            activitySignups.Add(new ActivitySignup { Activity = madrona, User = user1, SignupDateTime = DateTime.UtcNow });
            activitySignups.Add(new ActivitySignup { Activity = madrona, User = user2, SignupDateTime = DateTime.UtcNow });
            activitySignups.Add(new ActivitySignup { Activity = madrona, User = user3, SignupDateTime = DateTime.UtcNow });
            #endregion

            #region TaskSignups
            int i = 0;
            foreach (var task in tasks.Where(t => t.Activity == madrona))
            {
                for (var j = 0; j < i; j++)
                {
                    taskSignups.Add(new TaskSignup() { Task = task, User = users[j], Status = Areas.Admin.Features.Tasks.TaskStatus.Assigned.ToString() });
                }

                i = (i + 1) % users.Count;
            }
            _context.TaskSignups.AddRange(taskSignups);
            #endregion

            #region TennatContacts
            htb.OrganizationContacts.Add(new OrganizationContact { Contact = contacts.First(), Organization = htb, ContactType = 1 /*Primary*/ });
            #endregion

            #region Wrap Up DB  
            _context.ActivitySignup.AddRange(activitySignups);
            _context.SaveChanges();
            #endregion

        }

        private List<Contact> GetContacts()
        {
            var list = new List<Contact>();
            list.Add(new Contact { FirstName = "Bob", LastName = "Smith", Email = "BobSmith@mailinator.com", PhoneNumber = "999-888-7777" });
            list.Add(new Contact { FirstName = "George", LastName = "Leone", Email = "GeorgeLeone@mailinator.com", PhoneNumber = "999-888-7777" });
            return list;
        }

        #region Sample Data Helper methods
        private static T GetRandom<T>(List<T> list)
        {
            Random rand = new Random();
            return list[rand.Next(list.Count)];
        }

        private static List<AllReadyTask> GetSomeTasks(Activity activity, Organization tenant)
        {
            List<AllReadyTask> value = new List<AllReadyTask>();
            for (int i = 0; i < 5; i++)
            {
                //var tempId = _taskIdProvider.NextValue();
                value.Add(new AllReadyTask()
                {
                    Activity = activity,
                    Description = "Description of a very important task # " + i,
                    Name = "Task # " + i,
                    EndDateTime = DateTime.Now.AddDays(i),
                    StartDateTime = DateTime.Now.AddDays(i - 1),
                    Organization = tenant
                });
            }
            return value;
        }

        private Location CreateLocation(string address1, string city, string state, string postalCode)
        {
            Location ret = new Location();
            ret.Address1 = address1;
            ret.City = city;
            ret.State = state;
            ret.Country = "US";
            ret.PostalCode = _context.PostalCodes.FirstOrDefault(p => p.PostalCode == postalCode);
            ret.Name = "Humanitarian Toolbox location";
            ret.PhoneNumber = "1-425-555-1212";
            return ret;
        }

        private List<PostalCodeGeo> GetPostalCodes(IList<PostalCodeGeo> existingPostalCode)
        {
            var postalCodes = new List<PostalCodeGeo>();
            if (!existingPostalCode.Any(item => item.PostalCode == "98052")) postalCodes.Add(new PostalCodeGeo() { City = "Remond", State = "WA", PostalCode = "98052" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98004")) postalCodes.Add(new PostalCodeGeo() { City = "Bellevue", State = "WA", PostalCode = "98004" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98116")) postalCodes.Add(new PostalCodeGeo() { City = "Seattle", State = "WA", PostalCode = "98116" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98117")) postalCodes.Add(new PostalCodeGeo() { City = "Seattle", State = "WA", PostalCode = "98117" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98007")) postalCodes.Add(new PostalCodeGeo() { City = "Bellevue", State = "WA", PostalCode = "98007" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98027")) postalCodes.Add(new PostalCodeGeo() { City = "Issaquah", State = "WA", PostalCode = "98027" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98034")) postalCodes.Add(new PostalCodeGeo() { City = "Kirkland", State = "WA", PostalCode = "98034" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98033")) postalCodes.Add(new PostalCodeGeo() { City = "Kirkland", State = "WA", PostalCode = "98033" });
            if (!existingPostalCode.Any(item => item.PostalCode == "60505")) postalCodes.Add(new PostalCodeGeo() { City = "Aurora", State = "IL", PostalCode = "60505" });
            if (!existingPostalCode.Any(item => item.PostalCode == "60506")) postalCodes.Add(new PostalCodeGeo() { City = "Aurora", State = "IL", PostalCode = "60506" });
            if (!existingPostalCode.Any(item => item.PostalCode == "45231")) postalCodes.Add(new PostalCodeGeo() { City = "Cincinnati", State = "OH", PostalCode = "45231" });
            if (!existingPostalCode.Any(item => item.PostalCode == "45240")) postalCodes.Add(new PostalCodeGeo() { City = "Cincinnati", State = "OH", PostalCode = "45240" });
            return postalCodes;
        }

        private List<Location> GetLocations()
        {
            var geoService = new GeoService();

            var ret = new List<Location>()
      {
        CreateLocation("1 Microsoft Way", "Redmond", "WA", "98052"),
        CreateLocation("15563 Ne 31st St", "Redmond", "WA", "98052"),
        CreateLocation("700 Bellevue Way Ne", "Bellevue", "WA", "98004"),
        CreateLocation("1702 Alki Ave SW", "Seattle", "WA", "98116"),
        CreateLocation("8498 Seaview Pl NW", "Seattle", "WA", "98117"),
        CreateLocation("6046 W Lake Sammamish Pkwy Ne", "Redmond", "WA", "98052"),
        CreateLocation("7031 148th Ave Ne", "Redmond", "WA", "98052"),
        CreateLocation("2430 148th Ave SE", "Bellevue", "WA", "98007"),
        CreateLocation("2000 NW Sammamish Rd", "Issaquah", "WA", "98027"),
        CreateLocation("9703 Ne Juanita Dr", "Kirkland", "WA", "98034"),
        CreateLocation("25 Lakeshore Plaza Dr", "Kirkland", "Washington", "98033"),
        CreateLocation("633 Waverly Way", "Kirkland", "WA", "98033"),
    };

            return ret;
        }
        #endregion

        /// <summary>
        /// Creates a administrative user who can manage the inventory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task CreateAdminUser()
        {
            var user = await _userManager.FindByNameAsync(_settings.DefaultAdminUsername);
            if (user == null)
            {
                user = new ApplicationUser { UserName = _settings.DefaultAdminUsername, Email = _settings.DefaultAdminUsername, TimeZoneId = _generalSettings.DefaultTimeZone };
                user.EmailConfirmed = true;
                _userManager.CreateAsync(user, _settings.DefaultAdminPassword).Wait();
                _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, "SiteAdmin")).Wait();

                var user2 = new ApplicationUser { UserName = _settings.DefaultTenantUsername, Email = _settings.DefaultTenantUsername, TimeZoneId = _generalSettings.DefaultTimeZone };
                // For the sake of being able to exercise Tenant-specific stuff, we need to associate a tenant.
                user2.EmailConfirmed = true;
                await _userManager.CreateAsync(user2, _settings.DefaultAdminPassword);
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.UserType, "TenantAdmin"));
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.Tenant, _context.Organizations.First().Id.ToString()));

                var user3 = new ApplicationUser { UserName = _settings.DefaultUsername, Email = _settings.DefaultUsername, TimeZoneId = _generalSettings.DefaultTimeZone };
                user3.EmailConfirmed = true;
                await _userManager.CreateAsync(user3, _settings.DefaultAdminPassword);
            }
        }
    }
}
