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
using AllReady.Extensions;
using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class InitialRequestConfirmationsSentHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UpdateRequestStatusToPendingConfirmation()
        {
            var requestId = Guid.NewGuid();
            var message = new InitialRequestConfirmationsSent { ItineraryId = 1, RequestIds = new List<Guid> { requestId } };
            var itinerary = new Itinerary { Id = 1, Date = DateTime.UtcNow };

            Context.Requests.Add(new Request { RequestId = requestId });
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new InitialRequestConfirmationsSentHandler(Context, backgroundJobClient.Object);
            await sut.Handle(message);

            var result = Context.Requests.Single(x => x.RequestId == message.RequestIds.First());

            Assert.True(result.Status == RequestStatus.PendingConfirmation);
        }

        [Fact]
        public async Task ScheduleISendRequestConfirmationMessagesAWeekBeforeAnItineraryDateWithTheCorrectMethodSignatureAndTheCorrectDate()
        {
            var requestId = Guid.NewGuid();
            var message = new InitialRequestConfirmationsSent { ItineraryId = 1, RequestIds = new List<Guid> { requestId } };
            var itinerary = new Itinerary { Id = 1, Date = DateTime.UtcNow };

            Context.Requests.Add(new Request { RequestId = requestId });
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();
            
            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new InitialRequestConfirmationsSentHandler(Context, backgroundJobClient.Object);
            await sut.Handle(message);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job =>
                job.Type == typeof(ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate) &&
                job.Method.Name == nameof(ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate.SendSms) &&
                job.Args[0] == message.RequestIds &&
                (int)job.Args[1] == message.ItineraryId),
                It.Is<ScheduledState>(ss => ss.EnqueueAt.Date.AtNoon() == itinerary.Date.AddDays(-7).AtNoon())), Times.Once);
        }
    }
}