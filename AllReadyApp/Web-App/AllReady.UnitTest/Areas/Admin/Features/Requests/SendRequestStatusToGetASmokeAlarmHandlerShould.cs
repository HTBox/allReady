using AllReady.Areas.Admin.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class SendRequestStatusToGetASmokeAlarmHandlerShould : InMemoryContextTest
    {
        private readonly Guid requestId;
        private readonly string providerRequestId;

        public SendRequestStatusToGetASmokeAlarmHandlerShould()
        {
            requestId = Guid.NewGuid();
            providerRequestId = "providerRequestId";

            Context.Requests.Add(new Request
            {
                ProviderRequestId = providerRequestId,
                RequestId = requestId,
                Source = RequestSource.Api
            });
            Context.SaveChanges();
        }

        [Fact]
        public async Task NotQueueUpAnythingIfTheRequestStatusIsNotASupportedStatus()
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = requestId,
                NewStatus = RequestStatus.PendingConfirmation
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public async Task NotQueueAnythingIfTheRequestCannotBeFound()
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = Guid.NewGuid(),
                NewStatus = RequestStatus.Completed
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Theory]
        [InlineData(RequestStatus.Completed)]
        [InlineData(RequestStatus.Canceled)]
        [InlineData(RequestStatus.Assigned)]
        [InlineData(RequestStatus.Unassigned)]
        public async Task EnqueueAJobIfTheNewStatusIsASupportedStatus(RequestStatus newStatus)
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = requestId,
                NewStatus = newStatus
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Once);
        }

        [Theory]
        [InlineData(RequestStatus.Completed, GasaStatus.Installed, true)]
        [InlineData(RequestStatus.Canceled, GasaStatus.Canceled, true)]
        [InlineData(RequestStatus.Assigned, GasaStatus.InProgress, true)]
        [InlineData(RequestStatus.Unassigned, GasaStatus.New, false)]
        public async Task MapValuesCorrectlyToGasaValues(RequestStatus newStatus, string expectedGasaStatus, bool expectedAcceptance)
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = requestId,
                NewStatus = newStatus
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(
                    job => job.Method.Name == nameof(SendRequestStatusToGetASmokeAlarm.Send)
                    && (string)job.Args[0] == providerRequestId
                    && (string)job.Args[1] == expectedGasaStatus
                    && (bool)job.Args[2] == expectedAcceptance), It.IsAny<EnqueuedState>()), Times.Once);
        }
    }
}
