using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class SendInProgressStatusToGetASmokeAlarmHandlerShould : InMemoryContextTest
    {
        private const string firstProviderrequestid = "providerRequestId1";
        private const string secondProviderrequestid = "providerRequestId2";
        private readonly Guid firstRequestId = Guid.NewGuid();
        private readonly Guid secondRequestId = Guid.NewGuid();
        private readonly Mock<IBackgroundJobClient> backgroundJobClient;

        public SendInProgressStatusToGetASmokeAlarmHandlerShould()
        {
            backgroundJobClient = new Mock<IBackgroundJobClient>();
            Context.Requests.Add(new Request { RequestId = firstRequestId, ProviderRequestId = firstProviderrequestid, Source = RequestSource.Api });
            Context.Requests.Add(new Request { RequestId = secondRequestId, ProviderRequestId = secondProviderrequestid, Source = RequestSource.Api });
            Context.SaveChanges();
        }

        [Fact]
        public async Task EnqueueARequestToGasaWithAStatusOfInProgressAndAcceptanceOfTrueForEveryRequestIdOnTheNotification()
        {
            const bool expectedAcceptance = true;
            var notification = new RequestsAssignedToItinerary
            {
                ItineraryId = 1,
                RequestIds = new List<Guid>
                {
                    firstRequestId,
                    secondRequestId
                }
            };

            var sut = new SendInProgressStatusToGetASmokeAlarmHandler(Context, backgroundJobClient.Object);
            await sut.Handle(notification);

            backgroundJobClient.Verify(s => s.Create(It.Is<Job>(job => job.Method.Name == nameof(SendRequestStatusToGetASmokeAlarm.Send) && (string)job.Args[0] == firstProviderrequestid && (string)job.Args[1] == GasaStatus.InProgress && (bool)job.Args[2] == expectedAcceptance), It.IsAny<EnqueuedState>()), Times.Once);
            backgroundJobClient.Verify(s => s.Create(It.Is<Job>(job => job.Method.Name == nameof(SendRequestStatusToGetASmokeAlarm.Send) &&  (string)job.Args[0] == secondProviderrequestid && (string)job.Args[1] == GasaStatus.InProgress && (bool)job.Args[2] == expectedAcceptance), It.IsAny<EnqueuedState>()), Times.Once);
        }
    }
}
