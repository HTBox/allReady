using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Notifications;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class AddRequestsToItineraryCommandHandlerShould : InMemoryContextTest
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

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new AddRequestsToItineraryCommand { ItineraryId = 7 });

            Assert.False(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact]
        public async Task AbortOnNotFoundRequests()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new AddRequestsToItineraryCommand { ItineraryId = _theItinerary.Id, RequestIdsToAdd = new List<string> { "7" } });

            Assert.False(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact]
        public async Task NotAlterTheAlreadyAssignedRequests()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new AddRequestsToItineraryCommand
            {
                ItineraryId = _theItinerary.Id,
                RequestIdsToAdd = new List<string> { _assignedRequest.RequestId.ToString() }
            });

            Assert.True(succeded);
            Assert.Empty(Context.ItineraryRequests);
        }

        [Fact]
        public async Task AssignRequestsToTheItinerary()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new AddRequestsToItineraryCommand
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
        public async Task PublishRequestsAssignedToItinerary_WithTheCorrectParameters()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new AddRequestsToItineraryCommand
            {
                ItineraryId = _theItinerary.Id,
                RequestIdsToAdd = new List<string> {_notAssignedRequest.RequestId.ToString()}
            };

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            await handler.Handle(message);

            mockMediator.Verify(x => x.PublishAsync(It.Is<RequestsAssignedToItinerary>(y => y.ItineraryId == message.ItineraryId)));
        }

        [Fact]
        public async Task MaintainTheOrderingOfTheItineraryRequests()
        {
            var mockMediator = new Mock<IMediator>();
            Context.ItineraryRequests.Add(new ItineraryRequest { OrderIndex = 1, ItineraryId = _theItinerary.Id });
            Context.SaveChanges();

            var handler = new AddRequestsToItineraryCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new AddRequestsToItineraryCommand
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
