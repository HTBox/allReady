using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class EditItineraryCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };
            htb.Campaigns.Add(firePrev);

            var queenAnne = new Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { },
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary
            };

            var itinerary = new Itinerary
            {
                Event = queenAnne,
                Name = "1st Itinerary",
                Date = new DateTime(2016, 07, 01),
                UseStartAddressAsEndAddress = false,
                StartLocation = new Location {Address1 = "1st Itinerary Start Location"},
                EndLocation = new Location {Address1 = "1st Itinerary End Location"}
            };
            itinerary.EndLocation = itinerary.StartLocation;

            Context.Events.Add(queenAnne);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();
        }

        [Fact]
        public async Task AddsNewItineraryWhenItDoesNotExist()
        {
            var query = new EditItineraryCommand {  Itinerary = new ItineraryEditViewModel
            {
                EventId = 1,
                Name = "New",
                Date = DateTime.Now
            }};

            var sut = new EditItineraryCommandHandler(Context);
            var result = await sut.Handle(query);

            Assert.True(result > 0);
            Assert.Equal(2, Context.Itineraries.Count());

            var data = Context.Itineraries.Count(x => x.Id == result);

            Assert.True(data == 1);
        }

        [Fact]
        public async Task UpdatesItineraryWhenItExists()
        {
            var query = new EditItineraryCommand
            {
                Itinerary = new ItineraryEditViewModel
                {
                    Id = 1,
                    EventId = 1,
                    Name = "Updated",
                    Date = DateTime.Now
                }
            };

            var sut = new EditItineraryCommandHandler(Context);
            var result = await sut.Handle(query);

            Assert.True(result == 1);
            Assert.Equal(1, Context.Itineraries.Count());
        }

        [Fact]
        public async Task AddNewEndLocationWhenUseStartAddressAsEndAddressIsUnchecked()
        {
            const string startAddress = "#1 Address";
            const string endAddress = "#2 Address";

            var initialItinerary = new Itinerary
            {
                Id = 2,
                EventId = 1,
                Name = "SUT Itinerary",
                Date = DateTime.Now,
                UseStartAddressAsEndAddress = true,
                StartLocation = new Location
                {
                    Address1 = startAddress
                }
            };
            initialItinerary.EndLocation = initialItinerary.StartLocation;
            Context.Itineraries.Add(initialItinerary);
            Context.SaveChanges();
            var command = new EditItineraryCommand
            {
                Itinerary = new ItineraryEditViewModel
                {
                    Id = initialItinerary.Id,
                    Date = initialItinerary.Date,
                    UseStartAddressAsEndAddress = false,
                    StartAddress1 = initialItinerary.StartLocation.Address1,
                    EndAddress1 = endAddress
                }
            };

            var sut = new EditItineraryCommandHandler(Context);
            var result = await sut.Handle(command);

            var resultItinerary = Context.Itineraries
                .Include(i => i.StartLocation)
                .Include(i => i.EndLocation)
                .Single(i => i.Id == result);
            Assert.False(resultItinerary.UseStartAddressAsEndAddress);
            Assert.NotNull(resultItinerary.StartLocation);
            Assert.NotNull(resultItinerary.EndLocation);
            Assert.NotSame(resultItinerary.StartLocation, resultItinerary.EndLocation);
            Assert.Equal(startAddress, resultItinerary.StartLocation.Address1);
            Assert.Equal(endAddress, resultItinerary.EndLocation.Address1);
        }

        [Fact]
        public async Task DeleteEndLocationWhenUseStartAddressAsEndAddressIsChecked()
        {
            var initialItinerary = Context.Itineraries
                .Include(i => i.StartLocation)
                .Include(i => i.EndLocation)
                .Single();
            var endLocation = initialItinerary.EndLocation;
            var command = new EditItineraryCommand
            {
                Itinerary = new ItineraryEditViewModel
                {
                    Id = initialItinerary.Id,
                    Date = initialItinerary.Date,
                    UseStartAddressAsEndAddress = true,
                    StartAddress1 = initialItinerary.StartLocation.Address1,
                    EndAddress1 = initialItinerary.EndLocation.Address1
                }
            };

            var sut = new EditItineraryCommandHandler(Context);
            var result = await sut.Handle(command);

            var resultItinerary = Context.Itineraries
                .Include(i => i.StartLocation)
                .Include(i => i.EndLocation)
                .Single(i => i.Id == result);
            Assert.True(resultItinerary.UseStartAddressAsEndAddress);
            Assert.Same(resultItinerary.StartLocation, resultItinerary.EndLocation);
            Assert.DoesNotContain(endLocation, Context.Locations);
        }
    }
}

