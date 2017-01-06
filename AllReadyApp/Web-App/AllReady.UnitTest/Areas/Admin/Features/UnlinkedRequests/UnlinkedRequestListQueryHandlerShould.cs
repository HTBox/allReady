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

            Assert.IsType<List<UnlinkedRequestViewModel>>(result);
        }

        [Fact]
        public async Task NoRequestsFoundForOrganizationCriteria_ReturnsEmptyList()
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

            Assert.Empty(result);
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

            Assert.Empty(result);
        }

        [Fact]
        public async Task RequestsFoundWithCorrectOrgIdAndNullEventId_ReturnsExpectedCountOfViewModel()
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

            Assert.Equal(result.Count, 2);
        }

        [Fact]
        public async Task ReturnedViewModel_ContainsExpectedProperties()
        {
            var message = new UnlinkedRequestListQuery()
            {
                OrganizationId = 1
            };

            const string expectedName = "Test Event";
            const string expectedAddress = "Test address";
            const string expectedCity = "Test City";
            var expectedDate = DateTime.Now;
            var requests = new[]
            {
                new Request() { EventId  = null, OrganizationId = 1, Name = expectedName, Address = expectedAddress, City = expectedCity, DateAdded = expectedDate },
                new Request() { EventId  = 1, OrganizationId = 1, Name = "dummy name", Address = "dummy address", City = "dummy city", DateAdded = expectedDate.AddMinutes(1)}
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.SaveChanges();

            var sut = new UnlinkedRequestListQueryHandler(context);
            var result = await sut.Handle(message);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result.First().Name, expectedName);
            Assert.Equal(result.First().City, expectedCity);
            Assert.Equal(result.First().Address, expectedAddress);
            Assert.Equal(result.First().DateAdded, expectedDate);
        }
    }
}