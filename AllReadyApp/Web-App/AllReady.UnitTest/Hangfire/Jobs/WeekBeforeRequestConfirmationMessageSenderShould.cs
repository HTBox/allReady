using System;
using System.Collections.Generic;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;
using AllReady.Extensions;

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class WeekBeforeRequestConfirmationMessageSenderShould : InMemoryContextTest
    {
        //happy path
        [Fact]
        public void SendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsArePendingConfirmation_AndTodayIsEqualToOrGreaterThanSevenDaysBeforeTheItineariesDate()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation, Phone = "111-111-1111" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeUtcNow.Date.AddDays(8) };

            var requestIds = new List<Guid> { request.RequestId };
            var queueStorageService = new Mock<IQueueStorageService>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, queueStorageService.Object, backgroundJobClient.Object)
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(requestIds, itinerary.Id);

            queueStorageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var queueStorageService = new Mock<IQueueStorageService>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, queueStorageService.Object, Mock.Of<IBackgroundJobClient>());
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            queueStorageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotScheduleIDayBeforeRequestConfirmationMessageSender_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, Mock.Of<IQueueStorageService>(), backgroundJobClient.Object);
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var queueStorageService = new Mock<IQueueStorageService>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, queueStorageService.Object, Mock.Of<IBackgroundJobClient>());
            sut.SendSms(new List<Guid> { request.RequestId }, It.IsAny<int>());

            queueStorageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ScheduleIDayBeforeRequestConfirmationMessageSenderWithCorrectParameters_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeUtcNow.Date.AddDays(1) };

            var requestIds = new List<Guid> { request.RequestId };
            var queueStorageService = new Mock<IQueueStorageService>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, queueStorageService.Object, backgroundJobClient.Object) { DateTimeUtcNow = () => dateTimeUtcNow.Date };
            sut.SendSms(requestIds, itinerary.Id);

            backgroundJobClient.Verify(x =>
                x.Create(It.Is<Job>(job =>
                    job.Method.Name == nameof(IDayOfRequestConfirmationMessageSender.SendSms) &&
                    job.Args[0] == requestIds &&
                    (int)job.Args[1] == itinerary.Id),
                It.Is<ScheduledState>(ss => ss.EnqueueAt.Date.AtNoon() == itinerary.Date.AddDays(-1).AtNoon())),
            Times.Once);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsLessThanSevenDaysBeforeTheItinerariesDate()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeUtcNow.Date.AddDays(1) };

            var queueStorageService = new Mock<IQueueStorageService>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var sut = new WeekBeforeRequestConfirmationMessageSender(Context, queueStorageService.Object, backgroundJobClient.Object)
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(new List<Guid> { request.RequestId }, itinerary.Id);

            queueStorageService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void SendRequestConfirmationsToTheCorrectQueueNameWithTheCorrectMessageContents_WhenTodayIsEqualToOrGreaterThanTheItinerariesDate()
        {
        }

        [Fact]
        public void ScheduleIDayBeforeRequestConfirmationMessageSenderJob_WithCorrectRequestIdsAndItineraryId_OneDayBeforeItinerariesDateAtNoon()
        {
        }
    }
}