using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AllReady.Models;
using MediatR;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Areas.Admin.Features.Requests;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class RemoveRequestCommandHandlerShould : InMemoryContextTest
    {
        private static readonly Guid Request1Id = new Guid("de4f4639-86ea-419f-96c8-509defa4d9a3");
        private static readonly Guid Request2Id = new Guid("602b3f58-c8e0-4f59-82b0-f940c1aa1caa");
        private static readonly Guid Request3Id = new Guid("a7138311-66bf-4135-96a4-7177289c4933");

        protected override void LoadTestData()
        {
            var itinerary1 = new Itinerary
            {
                Id = 1,
                Name = "Test Itinerary 1"
            };

            var request1 = new Request
            {
                RequestId = Request1Id,
                Name = "Request 1",
                Latitude = 50.8225,
                Longitude = -0.1372,
                Status = RequestStatus.Assigned
            };

            var request2 = new Request
            {
                RequestId = Request2Id,
                Name = "Request 2",
                Latitude = 10.0000,
                Longitude = -5.0000,
                Status = RequestStatus.Completed
            };

            var request3 = new Request
            {
                RequestId = Request3Id,
                Name = "Request 3",
                Latitude = 10.0000,
                Longitude = -5.0000,
                Status = RequestStatus.Completed
            };

            var itineraryRequest1 = new ItineraryRequest
            {
                RequestId = Request1Id,
                Request = request1,
                Itinerary = itinerary1,
                OrderIndex = 1,
            };

            var itineraryRequest2 = new ItineraryRequest
            {
                RequestId = Request2Id,
                Request = request2,
                Itinerary = itinerary1,
                OrderIndex = 2,
            };

            var itineraryRequest3 = new ItineraryRequest
            {
                RequestId = Request3Id,
                Request = request3,
                Itinerary = itinerary1,
                OrderIndex = 3,
            };


            Context.Add(itinerary1);
            Context.Add(request1);
            Context.Add(request2);
            Context.Add(request3);
            Context.Add(itineraryRequest1);
            Context.Add(itineraryRequest2);
            Context.Add(itineraryRequest3);
            Context.SaveChanges();
        }

        [Fact]
        public async Task RemoveItineraryRequest()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            var succeded = await handler.Handle(new RemoveRequestCommand
                { ItineraryId = 1, RequestId = Request1Id });
            Assert.False(Context.ItineraryRequests.Any(x=>x.RequestId == Request1Id));
        }

        [Fact]
        public async Task Return_WhenNoIntineraryRequestsAreFoundByRequestId()
        {
            var mockMediator = new Mock<IMediator>(MockBehavior.Strict);

            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            await handler.Handle(new RemoveRequestCommand
                { ItineraryId = 1, RequestId = new Guid("0da127ec-e4b6-44df-abd3-8a9ffa685826") });
        }


        [Fact]
        public async Task Return_WhenRequestStatusIsCompleted()
        {
            var mockMediator = new Mock<IMediator>();
            
            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            await handler.Handle(new RemoveRequestCommand
                { ItineraryId = 1, RequestId = Request2Id });
            Assert.True(Context.ItineraryRequests.Any(x => x.RequestId == Request1Id));
        }

        [Fact]
        public async Task ReorderIntineraryRequestsCorrectly()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            await handler.Handle(new RemoveRequestCommand
                { ItineraryId = 1, RequestId = Request1Id });
            Assert.Equal(1, Context.ItineraryRequests.First(x => x.RequestId == Request2Id).OrderIndex);
            Assert.Equal(2, Context.ItineraryRequests.First(x => x.RequestId == Request3Id).OrderIndex);
        }

        [Fact]
        public async Task SetRequestStatusToUnassiged()
        {
            var mockMediator = new Mock<IMediator>();

            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            await handler.Handle(new RemoveRequestCommand
                { ItineraryId = 1, RequestId = Request1Id });
            Assert.Equal(RequestStatus.Unassigned, Context.Requests.First(x => x.RequestId == Request1Id).Status);
        }


        [Fact]	
        public async Task RemoveItineraryRequest_ShouldSendCorrectNotification()
        {
            var oldStatus = Context.Requests.First(x => x.RequestId == Request1Id).Status;

            var mockMediator = new Mock<IMediator>();
            var message = new RemoveRequestCommand { ItineraryId = 1, RequestId = Request1Id };
            var handler = new RemoveRequestCommandHandler(Context, mockMediator.Object);
            await handler.Handle(message);

            var newStatus = Context.Requests.First(x => x.RequestId == Request1Id).Status;

            mockMediator.Verify(x => x.PublishAsync(It.Is<RequestStatusChangedNotification>(y => y.RequestId == message.RequestId && y.OldStatus == oldStatus && y.NewStatus == newStatus)));
        }
    }
}
