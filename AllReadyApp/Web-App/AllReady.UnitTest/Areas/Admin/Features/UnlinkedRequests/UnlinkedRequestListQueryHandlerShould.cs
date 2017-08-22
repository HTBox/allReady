using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.UnlinkedRequests
{
    public class UnlinkedRequestListQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var message = new UnlinkedRequestListQuery();

            var requests = new[]
            {
                new Request() { EventId  = 1, OrganizationId = 1 }
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.IsType<UnlinkedRequestViewModel>(result);
        }

        [Fact]
        public async Task NoRequestsFoundForOrganizationCriteria_ReturnsEmptyRequestList()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            var requests = new[]
            {
                new Request() { EventId  = 1, OrganizationId = 2 }
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Empty(result.Requests);
        }

        [Fact]
        public async Task NoRequestsFoundWithNullEventId_ReturnsEmptyList()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            var requests = new[]
            {
                new Request() { EventId  = 1, OrganizationId = 1 },
                new Request() { EventId  = 2, OrganizationId = 2 }
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Empty(result.Requests);
        }

        [Fact]
        public async Task RequestsFoundWithCorrectOrgIdAndNullEventId_ReturnsExpectedRequestCount()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            var requests = new[]
            {
                new Request() { EventId  = null, OrganizationId = 1 },
                new Request() { EventId  = null, OrganizationId = 1 },
                new Request() { EventId  = null, OrganizationId = 2 },
                new Request() { EventId  = 1, OrganizationId = 1 },
                new Request() { EventId  = 1, OrganizationId = null },
                new Request() { EventId  = null, OrganizationId = null },
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Equal(2, result.Requests.Count);
        }

        [Fact]
        public async Task ReturnedViewModel_ContainsExpectedEventProperties()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            var events = new[]
            {
                new Event() { Id = 1, Name = "Event1", CampaignId = 1 },
                new Event() { Id = 2, Name = "Event2", CampaignId = 2 },
                new Event() { Id = 3, Name = "Event3", CampaignId = 3 },
                new Event() { Id = 4, Name = "Event1", CampaignId = 4 },
                new Event() { Id = 5, Name = "Event2", CampaignId = 5 },
                new Event() { Id = 6, Name = "Event3", CampaignId = 6 }
            };

            var campaigns = new[]
             {
               new Campaign() { Id = 1, Name = "Campaign1", ManagingOrganizationId = 1},
               new Campaign() { Id = 2, Name = "Campaign2", ManagingOrganizationId = 1},
               new Campaign() { Id = 3, Name = "Campaign3", ManagingOrganizationId = 1},
               new Campaign() { Id = 4, Name = "Campaign4", ManagingOrganizationId = 2},
               new Campaign() { Id = 5, Name = "Campaign5", ManagingOrganizationId = 3},
               new Campaign() { Id = 6, Name = "Campaign6", ManagingOrganizationId = 3},
            };

            var organizations = new[]
            {
                new Organization() {Id = 1, Name = "Organisation1"},
                new Organization() {Id = 2, Name = "Organisation2"},
                new Organization() {Id = 3, Name = "Organisation3"}
            };
            
            var context = Context;
            context.Events.AddRange(events);
            context.Campaigns.AddRange(campaigns);
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Equal(3, result.Events.Count);
            Assert.Equal(result.Events.Select(x => x.Value), new List<string> {"1", "2", "3"});
            Assert.Equal(result.Events.Select(x => x.Text), new List<string>()
            {
                "Organisation1 > Campaign1 > Event1",
                "Organisation1 > Campaign2 > Event2",
                "Organisation1 > Campaign3 > Event3",
            });
        }

        [Fact]
        public async Task ReturnedViewModel_ContainsExpectedRequestProperties()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            const string expectedName = "Test Event";
            const string expectedAddress = "Test address";
            const string expectedCity = "Test City";
            var expectedDate = DateTime.Now;
            var expectedRequestId = Guid.NewGuid();
            var requests = new[]
            {
                new Request() { RequestId = expectedRequestId, EventId  = null, OrganizationId = 1, Name = expectedName, Address = expectedAddress, City = expectedCity, DateAdded = expectedDate },
                new Request() { RequestId = Guid.NewGuid(), EventId  = 1, OrganizationId = 1, Name = "dummy name", Address = "dummy address", City = "dummy city", DateAdded = expectedDate.AddMinutes(1)}
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Single(result.Requests);
            Assert.Equal(result.Requests.First().Name, expectedName);
            Assert.Equal(result.Requests.First().City, expectedCity);
            Assert.Equal(result.Requests.First().Address, expectedAddress);
            Assert.Equal(result.Requests.First().DateAdded, expectedDate);
            Assert.Equal(result.Requests.First().Id, expectedRequestId);
        }
    }
}