using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class AddRequestsCommandHandlerAsyncShould : InMemoryContextTest
    {
        private Itinerary _theItinerary;
        private Request _assignedRequest;
        private Request _notAssignedRequest;

        protected override void LoadTestData()
        {
            _theItinerary = new Itinerary
            {
                Id = 1
            };

            _assignedRequest = new Request
            {
                RequestId = Guid.NewGuid(),
                Status = RequestStatus.Assigned,
            };

            _notAssignedRequest = new Request
            {
                RequestId = Guid.NewGuid(),
                Status = RequestStatus.Unassigned
            };

            Context.Itineraries.Add(_theItinerary);
            Context.Requests.Add(_assignedRequest);
            Context.Requests.Add(_notAssignedRequest);
            Context.SaveChanges();

        }

        [Fact]
        public async Task AbortOnNotFoundItinerary()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsCommandHandlerAsync(Context, mockMediator.Object);
            bool succeded = await handler.Handle(new AddRequestsCommand { ItineraryId = 7 });

            Assert.False(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact()]
        public async Task AbortOnNotFoundRequests()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsCommandHandlerAsync(Context, mockMediator.Object);
            bool succeded = await handler.Handle(new AddRequestsCommand { ItineraryId = _theItinerary.Id, RequestIdsToAdd = new List<string> { "7" } });

            Assert.False(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact()]
        public async Task NotAlterTheAlreadyAssignedRequests()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsCommandHandlerAsync(Context, mockMediator.Object);
            bool succeded = await handler.Handle(new AddRequestsCommand
            {
                ItineraryId = _theItinerary.Id,
                RequestIdsToAdd = new List<string> { _assignedRequest.RequestId.ToString() }
            });

            Assert.True(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact()]
        public async Task AssignRequestsToTheItinerary()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsCommandHandlerAsync(Context, mockMediator.Object);
            bool succeded = await handler.Handle(new AddRequestsCommand
            {
                ItineraryId = _theItinerary.Id,
                RequestIdsToAdd = new List<string> { _notAssignedRequest.RequestId.ToString() }
            });

            Assert.True(succeded);
            Assert.Equal(RequestStatus.Assigned, _notAssignedRequest.Status);
            Assert.Equal(1, Context.ItineraryRequests.Count());
            Assert.Equal(1, Context.ItineraryRequests.First().OrderIndex);
            Assert.Equal(_theItinerary.Id, Context.ItineraryRequests.First().ItineraryId);
            Assert.Equal(_notAssignedRequest.RequestId, Context.ItineraryRequests.First().RequestId);

        }

        [Fact]
        public async Task MaintainTheOrderingOfTheItineraryRequests()
        {
            var mockMediator = new Mock<IMediator>();
            Context.ItineraryRequests.Add(new ItineraryRequest { OrderIndex = 1, ItineraryId = _theItinerary.Id });
            Context.SaveChanges();

            var handler = new AddRequestsCommandHandlerAsync(Context, mockMediator.Object);
            bool succeded = await handler.Handle(new AddRequestsCommand
            {
                ItineraryId = _theItinerary.Id,
                RequestIdsToAdd = new List<string> { _notAssignedRequest.RequestId.ToString() }
            });

            Assert.True(succeded);
            Assert.Equal(2, Context.ItineraryRequests.Count());
            Assert.True(Context.ItineraryRequests.Any(ir => ir.OrderIndex == 2));
        }
    }
}
