using System;
using System.Collections.Generic;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesTheDayOfAnItineraryDateShould : InMemoryContextTest
    {
        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>());
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotSendSetRequstsToUnassignedCommand_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request { RequestId = Guid.NewGuid() };

            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object);
            sut.SendSms(new List<Guid> { Guid.NewGuid() }, It.IsAny<int>());

            mediator.Verify(x => x.Send(It.IsAny<IRequest>()), Times.Never);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.Assigned };

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>());
            sut.SendSms(new List<Guid> { request.RequestId }, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotSendSetRequstsToUnassignedCommand_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.Assigned };

            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object);
            sut.SendSms(new List<Guid> { request.RequestId }, It.IsAny<int>());
        }

        [Fact]
        public void SendRequestConfirmationsToTheCorrectPhoneNumberWithTheCorrectMessage_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsTheSameDateAsTheItineraryDate()
        {
            var dateTimeNow = DateTime.Today;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);
            var dateTimeUtcNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc);

            var requestorPhoneNumbers = new List<string> { "111-111-1111" };

            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified.Date, EventId = @event.Id, Event = @event };
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation, Phone = requestorPhoneNumbers[0] };

            var requestIds = new List<Guid> { request.RequestId };
            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>())
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(requestIds, itinerary.Id);

            smsSender.Verify(x => x.SendSmsAsync(requestorPhoneNumbers, "sorry you couldn't make it, we will reschedule."));
        }

        [Fact]
        public void PublishDayOfRequestConfirmationsSentWithCorrectParameters_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsNotTheSameDateAsTheItineraryDate()
        {
            var dateTimeNow = DateTime.Today;
            var dateTimeNowUnspecified = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Unspecified);
            var dateTimeUtcNow = DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc);

            var @event = new Event { Id = 1, TimeZoneId = "Eastern Standard Time" };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeNowUnspecified.Date.AddDays(1), EventId = @event.Id, Event = @event };
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation };
            
            var requestIds = new List<Guid> { request.RequestId };
            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object)
            {
                DateTimeUtcNow = () => dateTimeUtcNow.Date
            };
            sut.SendSms(requestIds, itinerary.Id);

            mediator.Verify(x => x.Publish(It.Is<DayOfRequestConfirmationsSent>(y => y.RequestIds.Contains(request.RequestId))));
        }
    }
}