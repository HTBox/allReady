using System;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using AllReady.Areas.Admin.Features.Requests;
    using AllReady.Hangfire.Jobs;

    using global::Hangfire;
    using global::Hangfire.Common;
    using global::Hangfire.States;

    using Moq;

    public class SendRequestStatusToGetASmokeAlarmHandlerShould : InMemoryContextTest
    {
        private readonly Guid requestId;

        public SendRequestStatusToGetASmokeAlarmHandlerShould()
        {
            this.requestId = Guid.NewGuid();
            this.Context.Requests.Add(new Request
            {
                RequestId = this.requestId,
                Source = RequestSource.Api
            });
            this.Context.SaveChanges();
        }

        [Fact]
        public async Task NotQueueUpAnythingIfTheRequestStatusIsNotASupportedStatus()
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = this.requestId,
                NewStatus = RequestStatus.PendingConfirmation
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(this.Context, backgroundJobClient.Object);

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
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(this.Context, backgroundJobClient.Object);

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
                RequestId = this.requestId,
                NewStatus = newStatus
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(this.Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Once);
        }

        [Theory]
        [InlineData(RequestStatus.Completed, "", true)]
        [InlineData(RequestStatus.Canceled, "canceled", true)]
        [InlineData(RequestStatus.Assigned, "in progress", true)]
        [InlineData(RequestStatus.Unassigned, "new", false)]
        public async Task MapRequestStatusValuesCorrectlyToGasaStatuses(RequestStatus newStatus, string expectedGasaStatus, bool expectedAcceptance)
        {
            var notification = new RequestStatusChangedNotification
            {
                RequestId = this.requestId,
                NewStatus = newStatus
            };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sut = new SendRequestStatusToGetASmokeAlarmHandler(this.Context, backgroundJobClient.Object);

            await sut.Handle(notification);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(
                    job => job.Method.Name == nameof(SendRequestStatusToGetASmokeAlarm.Send)
                    && (string)job.Args[1] == expectedGasaStatus
                    && (bool)job.Args[2] == expectedAcceptance), It.IsAny<EnqueuedState>()), Times.Once);
        }
    }
}
