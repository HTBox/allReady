using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Configuration;
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
        private readonly TimeZoneInfo _timeZone = TimeZoneInfo.Local;

        public SampleDataGenerator(AllReadyContext context, IOptions<SampleDataSettings> options, IOptions<GeneralSettings> generalSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _settings = options.Value;
            _generalSettings = generalSettings.Value;
            _userManager = userManager;
        }

        public async Task InsertTestData()
        {
            // Avoid polluting the database if there's already something in there.
            if (_context.Locations.Any() ||
                _context.Organizations.Any() ||
                _context.VolunteerTasks.Any() ||
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
            var volunteerTaskSignups = new List<VolunteerTaskSignup>();
            var events = new List<Event>();
            var eventSkills = new List<EventSkill>();
            var campaigns = new List<Campaign>();
            var volunteerTasks = new List<VolunteerTask>();
            var resources = new List<Resource>();
            var contacts = GetContacts();
            var skills = new List<Skill>();
            var eventManagers = new List<EventManager>();
            var campaignManagers = new List<CampaignManager>();

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
            //TODO: Campaign/Event/Task dates need to be set as a DateTimeOffset, offset to the correct timezone instead of UtcNow or DateTime.Today. 
            var firePreventionCampaign = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = organization,
                 Resources = resources,
                TimeZoneId = _timeZone.Id,
                StartDateTime = AdjustToTimezone(DateTimeOffset.Now.AddMonths(-1), _timeZone),
                EndDateTime = AdjustToTimezone(DateTimeOffset.Now.AddMonths(3), _timeZone),
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(firePreventionCampaign);

            var smokeDetectorCampaignGoal = new CampaignGoal
            {
                GoalType = GoalType.Numeric,
                NumericGoal = 10000,
                CurrentGoalLevel = 6722,
                Display = true,
                TextualGoal = "Total number of smoke detectors installed."
            };
            _context.CampaignGoals.Add(smokeDetectorCampaignGoal);

            var smokeDetectorCampaign = new Campaign
            {
                Name = "Working Smoke Detectors Save Lives",
                ManagingOrganization = organization,
                StartDateTime = AdjustToTimezone(DateTime.Today.AddMonths(-1), _timeZone),
                EndDateTime = AdjustToTimezone(DateTime.Today.AddMonths(1), _timeZone),
                CampaignGoals = new List<CampaignGoal> { smokeDetectorCampaignGoal },
                TimeZoneId = _timeZone.Id,
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(smokeDetectorCampaign);

            var financialCampaign = new Campaign
            {
                Name = "Everyday Financial Safety",
                ManagingOrganization = organization,
                TimeZoneId = _timeZone.Id,
                StartDateTime = AdjustToTimezone(DateTime.Today.AddMonths(-1), _timeZone),
                EndDateTime = AdjustToTimezone(DateTime.Today.AddMonths(1), _timeZone),
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(financialCampaign);

            var safetyKitCampaign = new Campaign
            {
                Name = "Simple Safety Kit Building",
                ManagingOrganization = organization,
                TimeZoneId = _timeZone.Id,
                StartDateTime = AdjustToTimezone(DateTime.Today.AddMonths(-1), _timeZone),
                EndDateTime = AdjustToTimezone(DateTime.Today.AddMonths(2), _timeZone),
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(safetyKitCampaign);

            var carSafeCampaign = new Campaign
            {
                Name = "Family Safety In the Car",
                ManagingOrganization = organization,
                TimeZoneId = _timeZone.Id,
                StartDateTime = AdjustToTimezone(DateTime.Today.AddMonths(-1), _timeZone),
                EndDateTime = AdjustToTimezone(DateTime.Today.AddMonths(2), _timeZone),
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(carSafeCampaign);

            var escapePlanCampaign = new Campaign
            {
                Name = "Be Ready to Get Out: Have a Home Escape Plan",
                ManagingOrganization = organization,
                TimeZoneId = _timeZone.Id,
                StartDateTime = AdjustToTimezone(DateTime.Today.AddMonths(-6), _timeZone),
                EndDateTime = AdjustToTimezone(DateTime.Today.AddMonths(6), _timeZone),
                Location = GetRandom(locations),
                Published = true
            };
            organization.Campaigns.Add(escapePlanCampaign);

            #endregion

            #region Event
            var queenAnne = new Event
            {
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddMonths(2), _timeZone),
                TimeZoneId = firePreventionCampaign.TimeZoneId,
                Location = GetRandom(locations),
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary,
            };
            queenAnne.VolunteerTasks = GetSomeVolunteerTasks(queenAnne, organization);
            var ask = new EventSkill { Skill = surgeon, Event = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            eventSkills.Add(ask);
            ask = new EventSkill { Skill = cprCertified, Event = queenAnne };
            queenAnne.RequiredSkills.Add(ask);
            eventSkills.Add(ask);
            volunteerTasks.AddRange(queenAnne.VolunteerTasks);

            var ballard = new Event
            {
                Name = "Ballard Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddMonths(2), _timeZone),
                TimeZoneId = firePreventionCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            ballard.VolunteerTasks = GetSomeVolunteerTasks(ballard, organization);
            volunteerTasks.AddRange(ballard.VolunteerTasks);
            var madrona = new Event
            {
                Name = "Madrona Fire Prevention Day",
                Campaign = firePreventionCampaign,
                StartDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(firePreventionCampaign.StartDateTime.AddMonths(2), _timeZone),
                TimeZoneId = firePreventionCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            madrona.VolunteerTasks = GetSomeVolunteerTasks(madrona, organization);
            volunteerTasks.AddRange(madrona.VolunteerTasks);
            var southLoopSmoke = new Event
            {
                Name = "Smoke Detector Installation and Testing-South Loop",
                Campaign = smokeDetectorCampaign,
                StartDateTime = AdjustToTimezone(smokeDetectorCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(smokeDetectorCampaign.EndDateTime, _timeZone),
                TimeZoneId = smokeDetectorCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            southLoopSmoke.VolunteerTasks = GetSomeVolunteerTasks(southLoopSmoke, organization);
            volunteerTasks.AddRange(southLoopSmoke.VolunteerTasks);
            var northLoopSmoke = new Event
            {
                Name = "Smoke Detector Installation and Testing-Near North Side",
                Campaign = smokeDetectorCampaign,
                StartDateTime = AdjustToTimezone(smokeDetectorCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(smokeDetectorCampaign.EndDateTime, _timeZone),
                TimeZoneId = smokeDetectorCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            northLoopSmoke.VolunteerTasks = GetSomeVolunteerTasks(northLoopSmoke, organization);
            volunteerTasks.AddRange(northLoopSmoke.VolunteerTasks);
            var dateTimeToday = DateTime.Today;
            var rentersInsurance = new Event
            {
                Name = "Renters Insurance Education Door to Door and a bag of chips",
                Description = "description for the win",
                Campaign = financialCampaign,
                StartDateTime = AdjustToTimezone(new DateTime(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day, 8, 0, 0), _timeZone),
                EndDateTime = AdjustToTimezone(new DateTime(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day, 16, 0, 0), _timeZone),
                TimeZoneId = financialCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Rally,
            };
            rentersInsurance.VolunteerTasks = GetSomeVolunteerTasks(rentersInsurance, organization);
            volunteerTasks.AddRange(rentersInsurance.VolunteerTasks);
            var rentersInsuranceEd = new Event
            {
                Name = "Renters Insurance Education Door to Door (woop woop)",
                Description = "another great description",
                Campaign = financialCampaign,
                StartDateTime = AdjustToTimezone(financialCampaign.StartDateTime.AddMonths(1).AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(financialCampaign.EndDateTime, _timeZone),
                TimeZoneId = financialCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            rentersInsuranceEd.VolunteerTasks = GetSomeVolunteerTasks(rentersInsuranceEd, organization);
            volunteerTasks.AddRange(rentersInsuranceEd.VolunteerTasks);
            var safetyKitBuild = new Event
            {
                Name = "Safety Kit Assembly Volunteer Day",
                Description = "Full day of volunteers building kits",
                Campaign = safetyKitCampaign,
                StartDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddMonths(1).AddDays(5), _timeZone),
                TimeZoneId = safetyKitCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            safetyKitBuild.VolunteerTasks = GetSomeVolunteerTasks(safetyKitBuild, organization);
            volunteerTasks.AddRange(safetyKitBuild.VolunteerTasks);

            var safetyKitHandout = new Event
            {
                Name = "Safety Kit Distribution Weekend",
                Description = "Handing out kits at local fire stations",
                Campaign = safetyKitCampaign,
                StartDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddMonths(1).AddDays(5), _timeZone),
                TimeZoneId = safetyKitCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            safetyKitHandout.VolunteerTasks = GetSomeVolunteerTasks(safetyKitHandout, organization);
            volunteerTasks.AddRange(safetyKitHandout.VolunteerTasks);
            var carSeatTest1 = new Event
            {
                Name = "Car Seat Testing-Naperville",
                Description = "Checking car seats at local fire stations after last day of school year",
                Campaign = carSafeCampaign,
                StartDateTime = AdjustToTimezone(carSafeCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(carSafeCampaign.StartDateTime.AddMonths(1).AddDays(5), _timeZone),
                TimeZoneId = carSafeCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            carSeatTest1.VolunteerTasks = GetSomeVolunteerTasks(carSeatTest1, organization);
            volunteerTasks.AddRange(carSeatTest1.VolunteerTasks);
            var carSeatTest2 = new Event
            {
                Name = "Car Seat and Tire Pressure Checking Volunteer Day",
                Description = "Checking those things all day at downtown train station parking",
                Campaign = carSafeCampaign,
                StartDateTime = AdjustToTimezone(carSafeCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(carSafeCampaign.StartDateTime.AddMonths(1).AddDays(5), _timeZone),
                TimeZoneId = carSafeCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            carSeatTest2.VolunteerTasks = GetSomeVolunteerTasks(carSeatTest2, organization);
            volunteerTasks.AddRange(carSeatTest2.VolunteerTasks);
            var homeFestival = new Event
            {
                Name = "Park District Home Safety Festival",
                Description = "At downtown park district(adjacent to pool)",
                Campaign = safetyKitCampaign,
                StartDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(safetyKitCampaign.StartDateTime.AddMonths(1), _timeZone),
                TimeZoneId = safetyKitCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            homeFestival.VolunteerTasks = GetSomeVolunteerTasks(homeFestival, organization);
            volunteerTasks.AddRange(homeFestival.VolunteerTasks);
            var homeEscape = new Event
            {
                Name = "Home Escape Plan Flyer Distribution",
                Description = "Handing out flyers door to door in several areas of town after school/ work hours.Streets / blocks will vary but number of volunteers.",
                Campaign = escapePlanCampaign,
                StartDateTime = AdjustToTimezone(escapePlanCampaign.StartDateTime.AddDays(1), _timeZone),
                EndDateTime = AdjustToTimezone(escapePlanCampaign.StartDateTime.AddMonths(7), _timeZone),
                TimeZoneId = escapePlanCampaign.TimeZoneId,
                Location = GetRandom(locations),
                EventType = EventType.Itinerary,
            };
            homeEscape.VolunteerTasks = GetSomeVolunteerTasks(homeEscape, organization);
            volunteerTasks.AddRange(homeEscape.VolunteerTasks);
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
                CategoryTag = "Partners",
                CampaignId = 1
            });
            resources.Add(new Resource
            {
                Name = "allReady Partner Name 2",
                Description = "allready Partner Description 2",
                PublishDateBegin = DateTime.Today.AddDays(-3),
                PublishDateEnd = DateTime.Today.AddDays(-1),
                MediaUrl = "",
                ResourceUrl = "",
                CategoryTag = "Partners",
                CampaignId = 1,
            });
            #endregion

            #region Insert into DB
            _context.Skills.AddRange(skills);
            _context.Contacts.AddRange(contacts);
            _context.EventSkills.AddRange(eventSkills);
            _context.Locations.AddRange(locations);
            _context.Organizations.AddRange(organizations);
            _context.VolunteerTasks.AddRange(volunteerTasks);
            _context.Campaigns.AddRange(campaigns);
            _context.Events.AddRange(events);
            _context.Resources.AddRange(resources);
            //_context.SaveChanges();
            #endregion

            #region Users for Events
            var username1 = $"{_settings.DefaultUsername}1.com";
            var username2 = $"{_settings.DefaultUsername}2.com";
            var username3 = $"{_settings.DefaultUsername}3.com";

            var user1 = new ApplicationUser
            {
                FirstName = "FirstName1",
                LastName = "LastName1",
                UserName = username1,
                Email = username1,
                EmailConfirmed = true,
                TimeZoneId = _timeZone.Id,
                PhoneNumber = "111-111-1111"
            };
            await _userManager.CreateAsync(user1, _settings.DefaultAdminPassword);
            users.Add(user1);

            var user2 = new ApplicationUser
            {
                FirstName = "FirstName2",
                LastName = "LastName2",
                UserName = username2,
                Email = username2,
                EmailConfirmed = true,
                TimeZoneId = _timeZone.Id,
                PhoneNumber = "222-222-2222"
            };
            await _userManager.CreateAsync(user2, _settings.DefaultAdminPassword);
            users.Add(user2);

            var user3 = new ApplicationUser
            {
                FirstName = "FirstName3",
                LastName = "LastName3",
                UserName = username3,
                Email = username3,
                EmailConfirmed = true,
                TimeZoneId = _timeZone.Id,
                PhoneNumber = "333-333-3333"
            };
            await _userManager.CreateAsync(user3, _settings.DefaultAdminPassword);
            users.Add(user3);
            #endregion

            #region Event Managers

            var eventManagerUser = new ApplicationUser
            {
                FirstName = "Event",
                LastName = "Manager",
                UserName = _settings.DefaultEventManagerUsername,
                Email = _settings.DefaultEventManagerUsername,
                EmailConfirmed = true,
                TimeZoneId = _timeZone.Id,
                PhoneNumber = "333-333-3333"
            };
            await _userManager.CreateAsync(eventManagerUser, _settings.DefaultAdminPassword);
            users.Add(eventManagerUser);

            var eventManager = new EventManager
            {
                Event = queenAnne,
                User = eventManagerUser
            };
            eventManagers.Add(eventManager);

            _context.EventManagers.AddRange(eventManagers);

            #endregion

            #region Campaign Managers

            var campaignManagerUser = new ApplicationUser
            {
                FirstName = "Campaign",
                LastName = "Manager",
                UserName = _settings.DefaultCampaignManagerUsername,
                Email = _settings.DefaultCampaignManagerUsername,
                EmailConfirmed = true,
                TimeZoneId = _timeZone.Id,
                PhoneNumber = "333-333-3333"
            };
            await _userManager.CreateAsync(campaignManagerUser, _settings.DefaultAdminPassword);
            users.Add(campaignManagerUser);

            var campaignManager = new CampaignManager
            {
                Campaign = firePreventionCampaign,
                User = campaignManagerUser
            };
            campaignManagers.Add(campaignManager);

            _context.CampaignManagers.AddRange(campaignManagers);

            #endregion

            #region TaskSignups
            var i = 0;
            foreach (var volunteerTask in volunteerTasks.Where(t => t.Event == madrona))
            {
                for (var j = 0; j < i; j++)
                {
                    volunteerTaskSignups.Add(new VolunteerTaskSignup { VolunteerTask = volunteerTask, User = users[j], Status = VolunteerTaskStatus.Assigned });
                }

                i = (i + 1) % users.Count;
            }
            _context.VolunteerTaskSignups.AddRange(volunteerTaskSignups);
            #endregion

            #region OrganizationContacts
            organization.OrganizationContacts.Add(new OrganizationContact { Contact = contacts.First(), Organization = organization, ContactType = 1 /*Primary*/ });
            #endregion

            #region Wrap Up DB
            _context.SaveChanges();
            #endregion
        }

        private DateTimeOffset AdjustToTimezone(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZone)
        {
            return new DateTimeOffset(dateTimeOffset.DateTime, timeZone.GetUtcOffset(dateTimeOffset.DateTime));
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

        private List<VolunteerTask> GetSomeVolunteerTasks(Event campaignEvent, Organization organization)
        {
            var volunteerTasks = new List<VolunteerTask>();
            for (var i = 0; i < 5; i++)
            {
                volunteerTasks.Add(new VolunteerTask
                {
                    Event = campaignEvent,
                    Description = "Description of a very important volunteerTask # " + i,
                    Name = "Task # " + i,
                    EndDateTime = AdjustToTimezone(DateTime.Today.AddHours(17).AddDays(i), _timeZone),
                    StartDateTime = AdjustToTimezone(DateTime.Today.AddHours(9).AddDays(campaignEvent.EventType == EventType.Itinerary ? i : i - 1), _timeZone),
                    Organization = organization,
                    NumberOfVolunteersRequired = 5
                });
            }
            return volunteerTasks;
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
            if (!existingPostalCode.Any(item => item.PostalCode == "98052")) postalCodes.Add(new PostalCodeGeo { City = "Redmond", State = "WA", PostalCode = "98052" });
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
                    FirstName = "FirstName4",
                    LastName = "LastName4",
                    UserName = _settings.DefaultAdminUsername,
                    Email = _settings.DefaultAdminUsername,
                    TimeZoneId = _timeZone.Id,
                    EmailConfirmed = true,
                    PhoneNumber = "444-444-4444"
                };
                await _userManager.CreateAsync(user, _settings.DefaultAdminPassword);
                await _userManager.AddClaimAsync(user, new Claim(Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)));

                var user2 = new ApplicationUser
                {
                    FirstName = "FirstName5",
                    LastName = "LastName5",
                    UserName = _settings.DefaultOrganizationUsername,
                    Email = _settings.DefaultOrganizationUsername,
                    TimeZoneId = _timeZone.Id,
                    EmailConfirmed = true,
                    PhoneNumber = "555-555-5555"
                };
                // For the sake of being able to exercise Organization-specific stuff, we need to associate a organization.
                await _userManager.CreateAsync(user2, _settings.DefaultAdminPassword);
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)));
                await _userManager.AddClaimAsync(user2, new Claim(Security.ClaimTypes.Organization, _context.Organizations.First().Id.ToString()));

                var user3 = new ApplicationUser
                {
                    FirstName = "FirstName6",
                    LastName = "LastName6",
                    UserName = _settings.DefaultUsername,
                    Email = _settings.DefaultUsername,
                    TimeZoneId = _timeZone.Id,
                    EmailConfirmed = true,
                    PhoneNumber = "666-666-6666"
                };
                await _userManager.CreateAsync(user3, _settings.DefaultAdminPassword);
            }
        }
    }
}
