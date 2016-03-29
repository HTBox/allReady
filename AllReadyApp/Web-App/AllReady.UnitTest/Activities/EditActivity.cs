using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Activities
{
    public class EditActivity : TestBase
    {
        [Fact]
        public void ActivityDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Organization htb = new Organization()
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();

            var vm = new ActivityDetailModel
            {
                CampaignId = 1,
                TimeZoneId = "Central Standard Time"
            };
            var query = new EditActivityCommand { Activity = vm };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.True(result > 0);

            var data = context.Activities.Count(_ => _.Id == result);
            Assert.True(data == 1);
        }

        [Fact]
        public void ExistingActivity()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Organization htb = new Organization()
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            Activity queenAnne = new Activity()
            {
                Id = 100,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>()
            };
            context.Organizations.Add(htb);
            context.Activities.Add(queenAnne);
            context.SaveChanges();

            const string NEW_NAME = "Some new name value";

            var startDateTime = new DateTime(2015, 7, 12, 4, 15, 0);
            var endDateTime = new DateTime(2015, 12, 7, 15, 10, 0);
            var vm = new ActivityDetailModel
            {
                CampaignId = queenAnne.CampaignId,
                CampaignName = queenAnne.Campaign.Name,
                Description = queenAnne.Description,
                EndDateTime = endDateTime,
                Id = queenAnne.Id,
                ImageUrl = queenAnne.ImageUrl,
                Location = null,
                Name = NEW_NAME,
                RequiredSkills = queenAnne.RequiredSkills,
                TimeZoneId = "Central Standard Time",
                StartDateTime = startDateTime,
                Tasks = null,
                OrganizationId = queenAnne.Campaign.ManagingOrganizationId,
                OrganizationName = queenAnne.Campaign.ManagingOrganization.Name,
                Volunteers = null
            };
            var query = new EditActivityCommand { Activity = vm };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(100, result); // should get back the activity id

            var data = context.Activities.Single(_ => _.Id == result);
            Assert.Equal(NEW_NAME, data.Name);

            Assert.Equal(2015, data.StartDateTime.Year);
            Assert.Equal(7, data.StartDateTime.Month);
            Assert.Equal(12, data.StartDateTime.Day);
            Assert.Equal(4, data.StartDateTime.Hour);
            Assert.Equal(15, data.StartDateTime.Minute);
            Assert.Equal(-5, data.StartDateTime.Offset.TotalHours);

            Assert.Equal(2015, data.EndDateTime.Year);
            Assert.Equal(12, data.EndDateTime.Month);
            Assert.Equal(7, data.EndDateTime.Day);
            Assert.Equal(15, data.EndDateTime.Hour);
            Assert.Equal(10, data.EndDateTime.Minute);
            Assert.Equal(-6, data.EndDateTime.Offset.TotalHours);
        }

        [Fact]
        public void ExistingActivityUpdateLocation()
        {
            PostalCodeGeo seattlePostalCode = new PostalCodeGeo() { City = "Seattle", PostalCode = "98117", State = "WA" };
            Location seattle = new Location()
            {
                Id = 1,
                Address1 = "123 Main Street",
                Address2 = "Unit 2",
                City = "Seattle",
                PostalCode = seattlePostalCode,
                Country = "USA",
                State = "WA",
                Name = "Organizer name",
                PhoneNumber = "555-555-5555"
            };

            var context = ServiceProvider.GetService<AllReadyContext>();
            Organization htb = new Organization()
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            Activity queenAnne = new Activity()
            {
                Id = 100,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = seattle,
                RequiredSkills = new List<ActivitySkill>()
            };
            context.Locations.Add(seattle);
            context.Organizations.Add(htb);
            context.Activities.Add(queenAnne);
            context.SaveChanges();

            var NEW_LOCATION = LocationExtensions.ToEditModel(new Location()
            {
                Address1 = "123 new address",
                Address2 = "new suite",
                PostalCode = new PostalCodeGeo() { City = "Bellevue", PostalCode = "98004", State = "WA" },
                City = "Bellevue",
                State = "WA",
                Country = "USA",
                Name = "New name",
                PhoneNumber = "New number"
            });

            var locationEdit = new ActivityDetailModel
            {
                CampaignId = queenAnne.CampaignId,
                CampaignName = queenAnne.Campaign.Name,
                Description = queenAnne.Description,
                EndDateTime = queenAnne.EndDateTime,
                Id = queenAnne.Id,
                ImageUrl = queenAnne.ImageUrl,
                Location = NEW_LOCATION,
                Name = queenAnne.Name,
                RequiredSkills = queenAnne.RequiredSkills,
                TimeZoneId = "Central Standard Time",
                StartDateTime = queenAnne.StartDateTime,
                Tasks = null,
                OrganizationId = queenAnne.Campaign.ManagingOrganizationId,
                OrganizationName = queenAnne.Campaign.ManagingOrganization.Name,
                Volunteers = null
            };

            var query = new EditActivityCommand { Activity = locationEdit };
            var handler = new EditActivityCommandHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(100, result); // should get back the activity id

            var data = context.Activities.Single(a => a.Id == result);
            Assert.Equal(data.Location.Address1, NEW_LOCATION.Address1);
            Assert.Equal(data.Location.Address2, NEW_LOCATION.Address2);
            Assert.Equal(data.Location.City, NEW_LOCATION.City);
            Assert.Equal(data.Location.PostalCode?.PostalCode, NEW_LOCATION.PostalCode);
            Assert.Equal(data.Location.State, NEW_LOCATION.State);
            Assert.Equal(data.Location.Country, NEW_LOCATION.Country);
            Assert.Equal(data.Location.PhoneNumber, NEW_LOCATION.PhoneNumber);
            Assert.Equal(data.Location.Name, NEW_LOCATION.Name);
        }
    }
}

