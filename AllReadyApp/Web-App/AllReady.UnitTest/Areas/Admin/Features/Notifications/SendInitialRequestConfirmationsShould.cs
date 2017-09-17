using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class SendInitialRequestConfirmationsShould : InMemoryContextTest
    {
        [Fact]
        public async Task SendSmsToTheCorrectPhoneNumbersWithTheCorrectMessage()
        {
            var notification = new RequestsAssignedToItinerary { ItineraryId = 1, RequestIds = new List<Guid> { Guid.NewGuid() }};

            var requestorPhoneNumbers = new List<string> { "111-111-1111" };
            var request = new Request { RequestId = notification.RequestIds.First(), Phone = requestorPhoneNumbers.First() };
            var itinerary = new Itinerary { Id = notification.ItineraryId, Date = DateTime.UtcNow };
            var message = $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please respond with ""Y"" to confirm this request or ""N"" to cancel this request.";

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var smsSender = new Mock<ISmsSender>();

            var sut = new SendInitialRequestConfirmationsHandler(Context, smsSender.Object, Mock.Of<IMediator>());
            await sut.Handle(notification);

            smsSender.Verify(x => x.SendSmsAsync(requestorPhoneNumbers, message));
        }

        [Fact]
        public async Task PublishInitialRequestConfirmationsSentWithCorrectValues()
        {
            var notification = new RequestsAssignedToItinerary { ItineraryId = 1, RequestIds = new List<Guid> { Guid.NewGuid() } };

            var request = new Request { RequestId = notification.RequestIds.First(), Phone = "111-111-1111" };
            var itinerary = new Itinerary { Id = notification.ItineraryId, Date = DateTime.UtcNow };

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var mediator = new Mock<IMediator>();

            var sut = new SendInitialRequestConfirmationsHandler(Context, Mock.Of<ISmsSender>(), mediator.Object);
            await sut.Handle(notification);

            mediator.Verify(x => x.PublishAsync(It.Is<InitialRequestConfirmationsSent>(y => y.RequestIds == notification.RequestIds && y.ItineraryId == itinerary.Id)));
        }
    }
}