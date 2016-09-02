using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AllReady.DataAccess
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
                _context.Events.Any() ||
                _context.EventSkills.Any() ||
                _context.Skills.Any() ||
                _context.Resources.Any())
            {
                return;
            }

            #region postalCodes
            var existingPostalCode = _context.PostalCodes.ToList();
            _context.PostalCodes.AddRange(GetPostalCodes(existingPostalCode));
            #endregion

            var organizations = new List<Organization>();
            var organizationSkills = new List<Skill>();
            var locations = GetLocations();
            var users = new List<ApplicationUser>();
            var taskSignups = new List<TaskSignup>();
            var events = new List<Event>();
            var eventSkills = new List<EventSkill>();
            var campaigns = new List<Campaign>();
            var tasks = new List<AllReadyTask>();
            var resources = new List<Resource>();
            var eventSignups = new List<EventSignup>();
            var contacts = GetContacts();
            var skills = new List<Skill>();

            #region Skills
            var medical = new Skill { Name = "Medical", Description = "specific enough, right?" };
            var cprCertified = new Skill { Name = "CPR Certified", ParentSkill = medical, Description = "ha ha ha ha, stayin alive" };
            var md = new Skill { Name = "MD", ParentSkill = medical, Description = "Trust me, I'm a doctor" };
            var surgeon = new Skill { Name = "Surgeon", ParentSkill = md, Description = "cut open; sew shut; play 18 holes" };
            skills.AddRange(new[] { medical, cprCertified, md, surgeon });
            #endregion

            #region Organization

            var organization = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Location = locations.FirstOrDefault(),
                Campaigns = new List<Campaign>(),
                OrganizationContacts = new List<OrganizationContact>(),
            };

            #endregion

            #region Organization Skills

            organizationSkills.Add(new Skill
            {
                Name = "Code Ninja",
                Description = "Ability to commit flawless code without review or testing",
                OwningOrganization = organization
            });

            #endregion

            #region Campaign

            var firePreventionCampaign = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = organization,
                TimeZoneId = "Central Standard Time",
                StartDateTime = DateTime.UtcNow.AddMonths(-1),
                EndDateTime = DateTime.UtcNow.AddMonths(3),
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(firePreventionCampaign);

            var smokeDetectorCampaignImpact = new CampaignImpact
            {
                ImpactType = ImpactType.Numeric,
                NumericImpactGoal = 10000,
                CurrentImpactLevel = 6722,
                Display = true,
                TextualImpactGoal = "Total number of smoke detectors installed."
            };
            _context.CampaignImpacts.Add(smokeDetectorCampaignImpact);

            var smokeDetectorCampaign = new Campaign
            {
                Name = "Working Smoke Detectors Save Lives",
                ManagingOrganization = organization,
                StartDateTime = DateTime.Today.AddMonths(-1),
                EndDateTime = DateTime.Today.AddMonths(1),
                CampaignImpact = smokeDetectorCampaignImpact,
                TimeZoneId = "Central Standard Time",
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(smokeDetectorCampaign);

            var financialCampaign = new Campaign
            {
                Name = "Everyday Financial Safety",
                ManagingOrganization = organization,
                TimeZoneId = "Central Standard Time",
                StartDateTime = DateTime.Today.AddMonths(-1),
                EndDateTime = DateTime.Today.AddMonths(1),
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(financialCampaign);

            var safetyKitCampaign = new Campaign
            {
                Name = "Simple Safety Kit Building",
                ManagingOrganization = organization,
                TimeZoneId = "Central Standard Time",
                StartDateTime = DateTime.Today.AddMonths(-1),
                EndDateTime = DateTime.Today.AddMonths(2),
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(safetyKitCampaign);

            var carSafeCampaign = new Campaign
            {
                Name = "Family Safety In the Car",
                ManagingOrganization = organization,
                TimeZoneId = "Central Standard Time",
                StartDateTime = DateTime.Today.AddMonths(-1),
                EndDateTime = DateTime.Today.AddMonths(2),
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(carSafeCampaign);

            var escapePlanCampaign = new Campaign
            {
                Name = "Be Ready to Get Out: Have a Home Escape Plan",
                ManagingOrganization = organization,
                TimeZoneId = "Central Standard Time",
                StartDateTime = DateTime.Today.AddMonths(-6),
                EndDateTime = DateTime.Today.AddMonths(6),
                Location = GetRandom(locations)
            };
            organization.Campaigns.Add(escapePlanCampaign);

            #endregion

            #region Event
            var queenAnne = new Event
            {
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = firePreventionCampaign.StartDateTime.AddDays(1),
                EndDateTime = firePreventionCampaign.StartDateTime.AddMonths(2),
                Location = GetRandom(locations),
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            queenAnne.Tasks = GetSomeTasks(queenAnne, organization);
            var ask = new EventSkill { Skill = surgeon, Event = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            eventSkills.Add(ask);
            ask = new EventSkill { Skill = cprCertified, Event = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            eventSkills.Add(ask);
            tasks.AddRange(queenAnne.Tasks);

            var ballard = new Event
            {
                Name = "Ballard Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = firePreventionCampaign.StartDateTime.AddDays(1),
                EndDateTime = firePreventionCampaign.StartDateTime.AddMonths(2),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            ballard.Tasks = GetSomeTasks(ballard, organization);
            tasks.AddRange(ballard.Tasks);
            var madrona = new Event
            {
                Name = "Madrona Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = firePreventionCampaign.StartDateTime.AddDays(1),
                EndDateTime = firePreventionCampaign.StartDateTime.AddMonths(2),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            madrona.Tasks = GetSomeTasks(madrona, organization);
            tasks.AddRange(madrona.Tasks);
            var southLoopSmoke = new Event
            {
                Name = "Smoke Detector Installation and Testing-South Loop",
                Campaign = smokeDetectorCampaign,
                StartDateTime = smokeDetectorCampaign.StartDateTime.AddDays(1),
                EndDateTime = smokeDetectorCampaign.EndDateTime,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            southLoopSmoke.Tasks = GetSomeTasks(southLoopSmoke, organization);
            tasks.AddRange(southLoopSmoke.Tasks);
            var northLoopSmoke = new Event
            {
                Name = "Smoke Detector Installation and Testing-Near North Side",
                Campaign = smokeDetectorCampaign,
                StartDateTime = smokeDetectorCampaign.StartDateTime.AddDays(1),
                EndDateTime = smokeDetectorCampaign.EndDateTime,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            northLoopSmoke.Tasks = GetSomeTasks(northLoopSmoke, organization);
            tasks.AddRange(northLoopSmoke.Tasks);
            var dateTimeToday = DateTime.Today;
            var rentersInsurance = new Event
            {
                Name = "Renters Insurance Education Door to Door and a bag of chips",
                Description = "description for the win",
                Campaign = financialCampaign,
                StartDateTime = new DateTime(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day, 8, 0, 0),
                EndDateTime = new DateTime(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day, 16, 0, 0),
                Location = GetRandom(locations),
                EventType = EventType.Rally,
                NumberOfVolunteersRequired = 1
            };
            rentersInsurance.Tasks = GetSomeTasks(rentersInsurance, organization);
            tasks.AddRange(rentersInsurance.Tasks);
            var rentersInsuranceEd = new Event
            {
                Name = "Renters Insurance Education Door to Door (woop woop)",
                Description = "another great description",
                Campaign = financialCampaign,
                StartDateTime = financialCampaign.StartDateTime.AddMonths(1).AddDays(1),
                EndDateTime = financialCampaign.EndDateTime,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            rentersInsuranceEd.Tasks = GetSomeTasks(rentersInsuranceEd, organization);
            tasks.AddRange(rentersInsuranceEd.Tasks);
            var safetyKitBuild = new Event
            {
                Name = "Safety Kit Assembly Volunteer Day",
                Description = "Full day of volunteers building kits",
                Campaign = safetyKitCampaign,
                StartDateTime = safetyKitCampaign.StartDateTime.AddDays(1),
                EndDateTime = safetyKitCampaign.StartDateTime.AddMonths(1).AddDays(5),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            safetyKitBuild.Tasks = GetSomeTasks(safetyKitBuild, organization);
            tasks.AddRange(safetyKitBuild.Tasks);

            var safetyKitHandout = new Event
            {
                Name = "Safety Kit Distribution Weekend",
                Description = "Handing out kits at local fire stations",
                Campaign = safetyKitCampaign,
                StartDateTime = safetyKitCampaign.StartDateTime.AddDays(1),
                EndDateTime = safetyKitCampaign.StartDateTime.AddMonths(1).AddDays(5),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            safetyKitHandout.Tasks = GetSomeTasks(safetyKitHandout, organization);
            tasks.AddRange(safetyKitHandout.Tasks);
            var carSeatTest1 = new Event
            {
                Name = "Car Seat Testing-Naperville",
                Description = "Checking car seats at local fire stations after last day of school year",
                Campaign = carSafeCampaign,
                StartDateTime = carSafeCampaign.StartDateTime.AddDays(1),
                EndDateTime = carSafeCampaign.StartDateTime.AddMonths(1).AddDays(5),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            carSeatTest1.Tasks = GetSomeTasks(carSeatTest1, organization);
            tasks.AddRange(carSeatTest1.Tasks);
            var carSeatTest2 = new Event
            {
                Name = "Car Seat and Tire Pressure Checking Volunteer Day",
                Description = "Checking those things all day at downtown train station parking",
                Campaign = carSafeCampaign,
                StartDateTime = carSafeCampaign.StartDateTime.AddDays(1),
                EndDateTime = carSafeCampaign.StartDateTime.AddMonths(1).AddDays(5),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            carSeatTest2.Tasks = GetSomeTasks(carSeatTest2, organization);
            tasks.AddRange(carSeatTest2.Tasks);
            var homeFestival = new Event
            {
                Name = "Park District Home Safety Festival",
                Description = "At downtown park district(adjacent to pool)",
                Campaign = safetyKitCampaign,
                StartDateTime = safetyKitCampaign.StartDateTime.AddDays(1),
                EndDateTime = safetyKitCampaign.StartDateTime.AddMonths(1),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            homeFestival.Tasks = GetSomeTasks(homeFestival, organization);
            tasks.AddRange(homeFestival.Tasks);
            var homeEscape = new Event
            {
                Name = "Home Escape Plan Flyer Distribution",
                Description = "Handing out flyers door to door in several areas of town after school/ work hours.Streets / blocks will vary but number of volunteers.",
                Campaign = escapePlanCampaign,
                StartDateTime = escapePlanCampaign.StartDateTime.AddDays(1),
                EndDateTime = escapePlanCampaign.StartDateTime.AddMonths(7),
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
                NumberOfVolunteersRequired = 1
            };
            homeEscape.Tasks = GetSomeTasks(homeEscape, organization);
            tasks.AddRange(homeEscape.Tasks);
            #endregion
            #region Link campaign and event

            firePreventionCampaign.Events = new List<Event> { queenAnne, ballard, madrona };
            smokeDetectorCampaign.Events = new List<Event> { southLoopSmoke, northLoopSmoke };
            financialCampaign.Events = new List<Event> { rentersInsurance, rentersInsuranceEd };
            safetyKitCampaign.Events = new List<Event> { safetyKitBuild, safetyKitHandout };
            carSafeCampaign.Events = new List<Event> { carSeatTest1, carSeatTest2 };
            escapePlanCampaign.Events = new List<Event> { homeFestival, homeEscape };

            #endregion
            #region Add Campaigns and Events
            organizations.Add(organization);
            campaigns.Add(firePreventionCampaign);
            campaigns.Add(smokeDetectorCampaign);
            campaigns.Add(financialCampaign);
            campaigns.Add(escapePlanCampaign);
            campaigns.Add(safetyKitCampaign);
            campaigns.Add(carSafeCampaign);

            events.AddRange(firePreventionCampaign.Events);
            events.AddRange(smokeDetectorCampaign.Events);
            events.AddRange(financialCampaign.Events);
            events.AddRange(escapePlanCampaign.Events);
            events.AddRange(safetyKitCampaign.Events);
            events.AddRange(carSafeCampaign.Events);
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
            _context.EventSkills.AddRange(eventSkills);
            _context.Locations.AddRange(locations);
            _context.Organizations.AddRange(organizations);
            _context.Tasks.AddRange(tasks);
            _context.Campaigns.AddRange(campaigns);
            _context.Events.AddRange(events);
            _context.Resources.AddRange(resources);
            //_context.SaveChanges();
            #endregion

            #region Users for Events
            var username1 = $"{_settings.DefaultUsername}1.com";
            var username2 = $"{_settings.DefaultUsername}2.com";
            var username3 = $"{_settings.DefaultUsername}3.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone, PhoneNumber = "111-111-1111" };
            _userManager.CreateAsync(user1, _settings.DefaultAdminPassword).GetAwaiter().GetResult();
            users.Add(user1);

            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone, PhoneNumber = "222-222-2222" };
            _userManager.CreateAsync(user2, _settings.DefaultAdminPassword).GetAwaiter().GetResult();
            users.Add(user2);

            var user3 = new ApplicationUser { UserName = username3, Email = username3, EmailConfirmed = true, TimeZoneId = _generalSettings.DefaultTimeZone, PhoneNumber = "333-333-3333" };
            _userManager.CreateAsync(user3, _settings.DefaultAdminPassword).GetAwaiter().GetResult();
            users.Add(user3);
            #endregion

            #region ActvitySignups
            eventSignups.Add(new EventSignup { Event = madrona, User = user1, SignupDateTime = DateTime.UtcNow });
            eventSignups.Add(new EventSignup { Event = madrona, User = user2, SignupDateTime = DateTime.UtcNow });
            eventSignups.Add(new EventSignup { Event = madrona, User = user3, SignupDateTime = DateTime.UtcNow });
            #endregion

            #region TaskSignups
            var i = 0;
            foreach (var task in tasks.Where(t => t.Event == madrona))
            {
                for (var j = 0; j < i; j++)
                {
                    taskSignups.Add(new TaskSignup { Task = task, User = users[j], Status = Areas.Admin.Features.Tasks.TaskStatus.Assigned.ToString() });
                }

                i = (i + 1) % users.Count;
            }
            _context.TaskSignups.AddRange(taskSignups);
            #endregion

            #region TennatContacts
            organization.OrganizationContacts.Add(new OrganizationContact { Contact = contacts.First(), Organization = organization, ContactType = 1 /*Primary*/ });
            #endregion

            #region Wrap Up DB
            _context.EventSignup.AddRange(eventSignups);
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
            var rand = new Random();
            return list[rand.Next(list.Count)];
        }

        private static List<AllReadyTask> GetSomeTasks(Event campaignEvent, Organization organization)
        {
            var value = new List<AllReadyTask>();
            for (var i = 0; i < 5; i++)
            {
                //var tempId = _taskIdProvider.NextValue();
                value.Add(new AllReadyTask
                {
                    Event = campaignEvent,
                    Description = "Description of a very important task # " + i,
                    Name = "Task # " + i,
                    EndDateTime = DateTime.Today.AddHours(17).AddDays(i),
                    StartDateTime = DateTime.Today.AddHours(9).AddDays(i - 1),
                    Organization = organization
                });
            }
            return value;
        }

        private static Location CreateLocation(string address1, string city, string state, string postalCode)
        {
            var ret = new Location
            {
                Address1 = address1,
                City = city,
                State = state,
                Country = "US",
                PostalCode = postalCode,
                Name = "Humanitarian Toolbox location",
                PhoneNumber = "1-425-555-1212"
            };
            return ret;
        }

        private List<PostalCodeGeo> GetPostalCodes(IList<PostalCodeGeo> existingPostalCode)
        {
            var postalCodes = new List<PostalCodeGeo>();
            if (!existingPostalCode.Any(item => item.PostalCode == "98052")) postalCodes.Add(new PostalCodeGeo { City = "Remond", State = "WA", PostalCode = "98052" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98004")) postalCodes.Add(new PostalCodeGeo { City = "Bellevue", State = "WA", PostalCode = "98004" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98116")) postalCodes.Add(new PostalCodeGeo { City = "Seattle", State = "WA", PostalCode = "98116" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98117")) postalCodes.Add(new PostalCodeGeo { City = "Seattle", State = "WA", PostalCode = "98117" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98007")) postalCodes.Add(new PostalCodeGeo { City = "Bellevue", State = "WA", PostalCode = "98007" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98027")) postalCodes.Add(new PostalCodeGeo { City = "Issaquah", State = "WA", PostalCode = "98027" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98034")) postalCodes.Add(new PostalCodeGeo { City = "Kirkland", State = "WA", PostalCode = "98034" });
            if (!existingPostalCode.Any(item => item.PostalCode == "98033")) postalCodes.Add(new PostalCodeGeo { City = "Kirkland", State = "WA", PostalCode = "98033" });
            if (!existingPostalCode.Any(item => item.PostalCode == "60505")) postalCodes.Add(new PostalCodeGeo { City = "Aurora", State = "IL", PostalCode = "60505" });
            if (!existingPostalCode.Any(item => item.PostalCode == "60506")) postalCodes.Add(new PostalCodeGeo { City = "Aurora", State = "IL", PostalCode = "60506" });
            if (!existingPostalCode.Any(item => item.PostalCode == "45231")) postalCodes.Add(new PostalCodeGeo { City = "Cincinnati", State = "OH", PostalCode = "45231" });
            if (!existingPostalCode.Any(item => item.PostalCode == "45240")) postalCodes.Add(new PostalCodeGeo { City = "Cincinnati", State = "OH", PostalCode = "45240" });
            return postalCodes;
        }

        private List<Location> GetLocations()
        {
            var ret = new List<Location>
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
                CreateLocation("633 Waverly Way", "Kirkland", "WA", "98033")
            };

            return ret;
        }
        #endregion

        /// <summary>
        /// Creates a administrative user who can manage the inventory.
        /// </summary>
        public async Task CreateAdminUser()
        {
            var user = await _userManager.FindByNameAsync(_settings.DefaultAdminUsername);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = _settings.DefaultAdminUsername,
                    Email = _settings.DefaultAdminUsername,
                    TimeZoneId = _generalSettings.DefaultTimeZone,
                    EmailConfirmed = true,
                    PhoneNumber = "444-444-4444"
                };
                _userManager.CreateAsync(user, _settings.DefaultAdminPassword).GetAwaiter().GetResult();
                _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, "SiteAdmin")).GetAwaiter().GetResult();

                var user2 = new ApplicationUser
                {
                    UserName = _settings.DefaultOrganizationUsername,
                    Email = _settings.DefaultOrganizationUsername,
                    TimeZoneId = _generalSettings.DefaultTimeZone,
                    EmailConfirmed = true,
                    PhoneNumber = "555-555-5555"
                };
                // For the sake of being able to exercise Organization-specific stuff, we need to associate a organization.
                await _userManager.CreateAsync(user2, _settings.DefaultAdminPassword);
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.UserType, "OrgAdmin"));
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.Organization, _context.Organizations.First().Id.ToString()));

                var user3 = new ApplicationUser
                {
                    UserName = _settings.DefaultUsername,
                    Email = _settings.DefaultUsername,
                    TimeZoneId = _generalSettings.DefaultTimeZone,
                    EmailConfirmed = true,
                    PhoneNumber = "666-666-6666"
                };
                await _userManager.CreateAsync(user3, _settings.DefaultAdminPassword);
            }
        }
    }
}
