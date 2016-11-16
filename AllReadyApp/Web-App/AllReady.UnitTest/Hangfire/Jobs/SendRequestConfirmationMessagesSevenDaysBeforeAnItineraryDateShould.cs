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

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDateShould : InMemoryContextTest
    {
        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, Mock.Of<IBackgroundJobClient>(), smsSender.Object);
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotScheduleISendRequestConfirmationMessagesADayBeforeAnItineraryDate_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, backgroundJobClient.Object, Mock.Of<ISmsSender>());
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            backgroundJobClient.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.Assigned };

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, Mock.Of<IBackgroundJobClient>(), smsSender.Object);
            sut.SendSms(new List<Guid> { request.RequestId }, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsNotSevenDaysBeforeTheItineraryDate()
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);
            var dateTimeUtcNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc);

            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified.Date.AddDays(1), EventId = @event.Id, Event = @event };
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation };

            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, backgroundJobClient.Object, smsSender.Object)
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(new List<Guid> { request.RequestId }, itinerary.Id);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void SendRequestConfirmationsToTheCorrectPhoneNumbersWithTheCorrectMessage_WhenRequestIdsMatchExistingRequests_AndThoseRequestsArePendingConfirmation_AndTodayIsSevenDaysBeforeTheItineraryDate()
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);
            var dateTimeUtcNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc);

            var requestorPhoneNumbers = new List<string> { "111-111-1111" };

            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified.Date.AddDays(7), EventId = @event.Id, Event = @event };
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation, Phone = requestorPhoneNumbers[0] };

            var requestIds = new List<Guid> { request.RequestId };
            var smsSender = new Mock<ISmsSender>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, backgroundJobClient.Object, smsSender.Object)
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(requestIds, itinerary.Id);

            var message = $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request.";
            smsSender.Verify(x => x.SendSmsAsync(requestorPhoneNumbers, message), Times.Once);
        }

        [Fact]
        public void ScheduleISendRequestConfirmationMessagesADayBeforeAnItineraryDateWithCorrectParameters_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsSevenDaysBeforeTheItineraryDate()
        {
            var dateTimeNow = DateTime.Now;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);
            var dateTimeUtcNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc);

            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified.Date.AddDays(7), EventId = @event.Id, Event = @event };
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation };

            var requestIds = new List<Guid> { request.RequestId };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(Context, backgroundJobClient.Object, Mock.Of<ISmsSender>())
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(requestIds, itinerary.Id);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job =>
                job.Type == typeof(ISendRequestConfirmationMessagesADayBeforeAnItineraryDate) &&
                job.Method.Name == nameof(ISendRequestConfirmationMessagesADayBeforeAnItineraryDate.SendSms) &&
                job.Args[0] == requestIds &&
                (int)job.Args[1] == itinerary.Id),
                It.Is<ScheduledState>(ss => ss.EnqueueAt.Date == itinerary.Date.AddDays(-1))), Times.Once);
        }
    }
}