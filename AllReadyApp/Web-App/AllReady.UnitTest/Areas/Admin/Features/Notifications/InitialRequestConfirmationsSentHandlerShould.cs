using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Hangfire.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;
using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class InitialRequestConfirmationsSentHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UpdateRequestStatusToPendingConfirmation()
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);

            var requestId = Guid.NewGuid();
            var message = new InitialRequestConfirmationsSent { ItineraryId = 1, RequestIds = new List<Guid> { requestId }};
            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified, EventId = @event.Id, Event = @event };

            Context.Requests.Add(new Request { RequestId = requestId });
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new InitialRequestConfirmationsSentHandler(Context, backgroundJobClient.Object);
            await sut.Handle(message);

            var result = Context.Requests.Single(x => x.RequestId == message.RequestIds.First());

            Assert.True(result.Status == RequestStatus.PendingConfirmation);
        }

        [Fact]
        public async Task ScheduleISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDateWithTheCorrectMethodSignatureAndTheCorrectDate()
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);

            var requestId = Guid.NewGuid();
            var message = new InitialRequestConfirmationsSent { ItineraryId = 1, RequestIds = new List<Guid> { requestId } };
            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified, EventId = @event.Id, Event = @event };

            Context.Requests.Add(new Request { RequestId = requestId });
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new InitialRequestConfirmationsSentHandler(Context, backgroundJobClient.Object);
            await sut.Handle(message);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job =>
                job.Type == typeof(ISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate) &&
                job.Method.Name == nameof(ISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate.SendSms) &&
                job.Args[0] == message.RequestIds &&
                (int)job.Args[1] == message.ItineraryId),
                It.Is<ScheduledState>(ss => ss.EnqueueAt.Date.AddHours(12) == itinerary.Date.Date.AddDays(-7).AddHours(12))), Times.Once);
        }
    }
}