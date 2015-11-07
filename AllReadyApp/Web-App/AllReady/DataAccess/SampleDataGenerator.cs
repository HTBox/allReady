using Microsoft.AspNet.Identity;
using Microsoft.Framework.Configuration;
using AllReady.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public class SampleDataGenerator
    {
        private static IConfiguration _configuration;
        private AllReadyContext _context;
        private UserManager<ApplicationUser> _userManager;

        public SampleDataGenerator(AllReadyContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        //private static ITaskIdProvider _taskIdProvider = new TaskIdProvider();
        public void InsertTestData()
        {
            // Avoid polluting the database if there's already something in there.
            if (_context.Locations.Any() ||
                _context.Tenants.Any() ||
                _context.Tasks.Any() ||
                _context.Campaigns.Any() ||
                _context.Activities.Any() ||
                _context.ActivitySkills.Any() ||
                _context.Skills.Any() ||
                _context.Resources.Any())
            {
                // Attempt to populate CampaignImpactTypes:
                InsertCampaignImpactTypes(_context);
                _context.SaveChanges();
                return;
            }
            // new up some data
            List<Tenant> tenants = new List<Tenant>();

            #region postalCodes
            var existingPostalCode = _context.PostalCodes.ToList();
            _context.PostalCodes.AddRange(GetPostalCodes(existingPostalCode));
            //_context.SaveChanges();
            #endregion

            #region Skills
            var skills = new List<Skill>();
            var existingSkills = _context.Skills.ToList();
            var medical = GetSkill(skills, existingSkills, "Medical", null, "specific enough, right?");
            var cprCertified = GetSkill(skills, existingSkills, "CPR Certified", medical, "ha ha ha ha, stayin alive");
            var md = GetSkill(skills, existingSkills, "MD", medical, "Trust me, I'm a doctor");
            var surgeon = GetSkill(skills, existingSkills, "Surgeon", md, "cut open; sew shut; play 18 holes");
            _context.Skills.AddRange(skills);
            //_context.SaveChanges();
            #endregion

            List<Location> locations = GetLocations();
            List<ApplicationUser> users = new List<ApplicationUser>();
            List<TaskSignup> taskSignups = new List<TaskSignup>();
            List<Activity> activities = new List<Activity>();
            List<ActivitySkill> activitySkills = new List<ActivitySkill>();
            List<Campaign> campaigns = new List<Campaign>();
            List<AllReadyTask> tasks = new List<AllReadyTask>();
            List<Resource> resources = new List<Resource>();
            List<ActivitySignup> activitySignups = new List<ActivitySignup>();

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
                EndDateTimeUtc = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Tenant = htb,
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
                StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 14, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Tenant = htb

            };
            ballard.Tasks = GetSomeTasks(ballard, htb);
            tasks.AddRange(ballard.Tasks);
            Activity madrona = new Activity()
            {
                Name = "Madrona Fire Prevention Day",
                StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 14, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Tenant = htb
            };
            madrona.Tasks = GetSomeTasks(madrona, htb);
            tasks.AddRange(madrona.Tasks);
            Activity southLoopSmoke = new Activity()
            {
                Name = "Smoke Detector Installation and Testing-South Loop",
                StartDateTimeUtc = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 17, 0, 0).ToUniversalTime(),
                Location = GetRandom<Location>(locations),
                Tenant = htb
            };
            southLoopSmoke.Tasks = GetSomeTasks(southLoopSmoke, htb);
            tasks.AddRange(southLoopSmoke.Tasks);
            Activity northLoopSmoke = new Activity()
            {
                Name = "Smoke Detector Installation and Testing-Near North Side",
                StartDateTimeUtc = new DateTime(2015, 7, 6, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 17, 0, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 12, 17, 0, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 10, 15, 30, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 11, 19, 30, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 11, 16, 30, 0).ToUniversalTime(),
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
                EndDateTimeUtc = new DateTime(2015, 12, 15, 20, 30, 0).ToUniversalTime(),
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

            _context.ActivitySkills.AddRange(activitySkills);
            _context.Locations.AddRange(locations);
            _context.Tenants.AddRange(tenants);
            _context.Tasks.AddRange(tasks);
            _context.Campaigns.AddRange(campaigns);
            _context.Activities.AddRange(activities);
            _context.Resources.AddRange(resources);
            //_context.SaveChanges();
            #endregion

            #region Users for Activities
            var username1 = $"{_configuration["SampleData:DefaultUsername"]}1.com";
            var username2 = $"{_configuration["SampleData:DefaultUsername"]}2.com";
            var username3 = $"{_configuration["SampleData:DefaultUsername"]}3.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            _userManager.CreateAsync(user1, _configuration["SampleData:DefaultAdminPassword"]).Wait();
            users.Add(user1);
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            _userManager.CreateAsync(user2, _configuration["SampleData:DefaultAdminPassword"]).Wait();
            users.Add(user2);
            var user3 = new ApplicationUser { UserName = username3, Email = username3, EmailConfirmed = true };
            _userManager.CreateAsync(user3, _configuration["SampleData:DefaultAdminPassword"]).Wait();
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
                    taskSignups.Add(new TaskSignup() { Task = task, User = users[j] });
                }

                i = (i + 1) % users.Count;
            }
            _context.TaskSignups.AddRange(taskSignups);
            #endregion

            #region Wrap Up DB  
            _context.ActivitySignup.AddRange(activitySignups);
            _context.SaveChanges();
            #endregion

        }

        /// <summary>
        /// Split this off from the InsertTestData function so we can call this
        /// even if the "anti-database-pollution" logic bails due to pre-existing
        /// database records. The reason for this is that we are preventing
        /// duplicate records already.
        /// </summary>
        /// <param name="_context"></param>
        private static void InsertCampaignImpactTypes(AllReadyContext _context)
        {
            var campaignImpactTypes = new List<CampaignImpactType>();
            var existingImpactTypes = _context.CampaignImpactTypes.ToList();
            var numeric = GetImpactType(campaignImpactTypes, existingImpactTypes, "Numeric");
            var textual = GetImpactType(campaignImpactTypes, existingImpactTypes, "Textual");
            _context.CampaignImpactTypes.AddRange(campaignImpactTypes);
        }

        private static Skill GetSkill(List<Skill> skills, List<Skill> existingSkills, string skillName, Skill parentSkill = null, string description = null)
        {
            Skill skill;
            if (existingSkills.Any(item => item.Name == skillName))
            {
                skill = existingSkills.Single(item => item.Name == skillName);
            }
            else
            {
                skill = new Skill { Name = skillName };
                if (parentSkill != null)
                {
                    skill.ParentSkill = parentSkill;
                }
                skills.Add(skill);
            }
            skill.Description = description;
            return skill;
        }

        private static CampaignImpactType GetImpactType(List<CampaignImpactType> types, List<CampaignImpactType> existingTypes, string typeName)
        {
            CampaignImpactType impactType;
            if (existingTypes.Any(item => item.ImpactType == typeName))
            {
                impactType = existingTypes.Single(item => item.ImpactType == typeName);
            }
            else
            {
                impactType = new CampaignImpactType { ImpactType = typeName };

                types.Add(impactType);
            }
            return impactType;
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
                //var tempId = _taskIdProvider.NextValue();
                value.Add(new AllReadyTask()
                {
                    Activity = activity,
                    Description = "Description of a very important task # " + i,
                    Name = "Task # " + i,
                    EndDateTimeUtc = DateTime.Now.AddDays(i),
                    StartDateTimeUtc = DateTime.Now.AddDays(i - 1),
                    Tenant = tenant
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
            var user = await _userManager.FindByNameAsync(_configuration["SampleData:DefaultAdminUsername"]);
            if (user == null)
            {
                user = new ApplicationUser { UserName = _configuration["SampleData:DefaultAdminUsername"], Email = _configuration["SampleData:DefaultAdminUsername"] };
                user.EmailConfirmed = true;
                _userManager.CreateAsync(user, _configuration["SampleData:DefaultAdminPassword"]).Wait();
                _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, "SiteAdmin")).Wait();

                var user2 = new ApplicationUser { UserName = _configuration["SampleData:DefaultTenantUsername"], Email = _configuration["SampleData:DefaultTenantUsername"] };
                // For the sake of being able to exercise Tenant-specific stuff, we need to associate a tenant.
                user2.EmailConfirmed = true;
                await _userManager.CreateAsync(user2, _configuration["SampleData:DefaultAdminPassword"]);
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.UserType, "TenantAdmin"));
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.Tenant, _context.Tenants.First().Id.ToString()));

                var user3 = new ApplicationUser { UserName = _configuration["SampleData:DefaultUsername"], Email = _configuration["SampleData:DefaultUsername"] };
                user3.EmailConfirmed = true;
                await _userManager.CreateAsync(user3, _configuration["SampleData:DefaultAdminPassword"]);
            }
        }
    }
}
