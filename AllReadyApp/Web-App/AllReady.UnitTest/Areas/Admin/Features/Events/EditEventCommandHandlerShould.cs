using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using AllReady.Providers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class EditEventCommandHandlerShould : TestBase
    {
        [Fact]
        public async Task EventDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);

            context.Organizations.Add(htb);
            context.SaveChanges();

            var vm = new EventEditViewModel
            {
                CampaignId = 1,
                TimeZoneId = "Central Standard Time"
            };

            var query = new EditEventCommand { Event = vm };
            var handler = new EditEventCommandHandler(context);
            var result = await handler.Handle(query);
            Assert.True(result > 0);

            var data = context.Events.Count(_ => _.Id == result);
            Assert.True(data == 1);
        }

        [Fact]
        public async Task ExistingEvent()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);

            var queenAnne = new Event
            {
                Id = 100,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>()
            };
            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);
            context.SaveChanges();

            const string newName = "Some new name value";

            var startDateTime = new DateTime(2015, 7, 12, 4, 15, 0);
            var endDateTime = new DateTime(2015, 12, 7, 15, 10, 0);
            var vm = new EventEditViewModel
            {
                CampaignId = queenAnne.CampaignId,
                CampaignName = queenAnne.Campaign.Name,
                Description = queenAnne.Description,
                EndDateTime = endDateTime,
                Id = queenAnne.Id,
                ImageUrl = queenAnne.ImageUrl,
                Location = null,
                Name = newName,
                RequiredSkills = queenAnne.RequiredSkills,
                TimeZoneId = "Central Standard Time",
                StartDateTime = startDateTime,
                OrganizationId = queenAnne.Campaign.ManagingOrganizationId,
                OrganizationName = queenAnne.Campaign.ManagingOrganization.Name,
            };
            var query = new EditEventCommand { Event = vm };
            var handler = new EditEventCommandHandler(context);
            var result = await handler.Handle(query);
            Assert.Equal(100, result); // should get back the event id

            var data = context.Events.Single(_ => _.Id == result);
            Assert.Equal(newName, data.Name);

            Assert.Equal(2015, data.StartDateTime.Year);
            Assert.Equal(7, data.StartDateTime.Month);
            Assert.Equal(12, data.StartDateTime.Day);
            Assert.Equal(4, data.StartDateTime.Hour);
            Assert.Equal(15, data.StartDateTime.Minute);


            Assert.Equal(2015, data.EndDateTime.Year);
            Assert.Equal(12, data.EndDateTime.Month);
            Assert.Equal(7, data.EndDateTime.Day);
            Assert.Equal(15, data.EndDateTime.Hour);
            Assert.Equal(10, data.EndDateTime.Minute);

        }

        [Fact]
        public async Task ExistingEventUpdateLocation()
        {
            const string seattlePostalCode = "98117";

            var seattle = new Location
            {
                Id = 12,
                Address1 = "123 Main Street",
                Address2 = "Unit 2",
                City = "Seattle",
                PostalCode = seattlePostalCode,
                Country = "USA",
                State = "WA",
                Name = "Organizer name",
                PhoneNumber = "555-555-5555"
            };

            var htb = new Organization
            {
                Id = 123,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Central Standard Time"
            };
            htb.Campaigns.Add(firePrev);

            var queenAnne = new Event
            {
                Id = 100,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = seattle,
                RequiredSkills = new List<EventSkill>()
            };

            var context = ServiceProvider.GetService<AllReadyContext>();
            context.Locations.Add(seattle);
            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);
            context.SaveChanges();

            var newLocation = new Location
            {
                Address1 = "123 new address",
                Address2 = "new suite",
                PostalCode = "98004",
                City = "Bellevue",
                State = "WA",
                Country = "USA",
                Name = "New name",
                PhoneNumber = "New number"
            }.ToEditModel();

            var locationEdit = new EventEditViewModel
            {
                CampaignId = queenAnne.CampaignId,
                CampaignName = queenAnne.Campaign.Name,
                Description = queenAnne.Description,
                EndDateTime = queenAnne.EndDateTime,
                Id = queenAnne.Id,
                ImageUrl = queenAnne.ImageUrl,
                Location = newLocation,
                Name = queenAnne.Name,
                RequiredSkills = queenAnne.RequiredSkills,
                TimeZoneId = "Central Standard Time",
                StartDateTime = queenAnne.StartDateTime,
                OrganizationId = queenAnne.Campaign.ManagingOrganizationId,
                OrganizationName = queenAnne.Campaign.ManagingOrganization.Name,
            };

            var query = new EditEventCommand { Event = locationEdit };
            var handler = new EditEventCommandHandler(context);
            var result = await handler.Handle(query);
            Assert.Equal(100, result); // should get back the event id

            var data = context.Events.Single(a => a.Id == result);
            Assert.Equal(data.Location.Address1, newLocation.Address1);
            Assert.Equal(data.Location.Address2, newLocation.Address2);
            Assert.Equal(data.Location.City, newLocation.City);
            Assert.Equal(data.Location.PostalCode, newLocation.PostalCode);
            Assert.Equal(data.Location.State, newLocation.State);
            Assert.Equal(data.Location.Country, newLocation.Country);
            Assert.Equal(data.Location.PhoneNumber, newLocation.PhoneNumber);
            Assert.Equal(data.Location.Name, newLocation.Name);
        }

        [Fact]
        public async Task ModelIsCreated()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            // Adding an event requires a campaign for a organization ID and an event to match that in the command
            context.Campaigns.Add(new Campaign { Id = 1, TimeZoneId = "Central Standard Time" });
            context.Events.Add(new Event { Id = 1 });
            context.SaveChanges();

            var sut = new EditEventCommandHandler(context);
            var actual = await sut.Handle(new EditEventCommand { Event = new EventEditViewModel { CampaignId = 1, Id = 1, TimeZoneId = "Central Standard Time" } });
            Assert.Equal(1, actual);
        }

    }
}
