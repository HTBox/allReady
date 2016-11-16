using System;
using System.Collections.Generic;
using AllReady.Hangfire.Jobs;
using AllReady.Hangfire.MediatR;
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
            var request = new Request {RequestId = Guid.NewGuid()};

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>());
            sut.SendSms(new List<Guid> {Guid.NewGuid()}, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotSendSetRequstsToUnassignedCommand_WhenRequestIdsDoNotMatchExistingRequests()
        {
            var request = new Request {RequestId = Guid.NewGuid()};

            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object);
            sut.SendSms(new List<Guid> {Guid.NewGuid()}, It.IsAny<int>());

            mediator.Verify(x => x.Send(It.IsAny<IRequest>()), Times.Never);
        }

        [Fact]
        public void NotSendRequestConfirmations_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request {RequestId = Guid.NewGuid(), Status = RequestStatus.Assigned};

            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>());
            sut.SendSms(new List<Guid> {request.RequestId}, It.IsAny<int>());

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotSendSetRequstsToUnassignedCommand_WhenRequestIdsMatchExistingRequests_AndThoseRequestsDoNotHaveAStatusOfPendingConfirmation()
        {
            var request = new Request {RequestId = Guid.NewGuid(), Status = RequestStatus.Assigned};

            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object);
            sut.SendSms(new List<Guid> {request.RequestId}, It.IsAny<int>());
        }

        [Fact]
        public void SendSetRequstsToUnassignedCommandWithCorrectParameters_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsNotTheSameDateAsTheItineraryDate()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var request = new Request {RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation};
            var itinerary = new Itinerary {Id = 1, Date = dateTimeUtcNow.Date.AddDays(1)};

            var requestIds = new List<Guid> {request.RequestId};
            var mediator = new Mock<IMediator>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, Mock.Of<ISmsSender>(), mediator.Object) {DateTimeUtcNow = () => dateTimeUtcNow.Date};
            sut.SendSms(requestIds, itinerary.Id);

            mediator.Verify(x => x.Send(It.Is<SetRequestsToUnassignedCommand>(y => y.RequestIds.Contains(request.RequestId))));
        }

        [Fact]
        public void SendRequestConfirmationsWithCorrectParameters_WhenRequestIdsMatchExistingRequests_AndThoseRequestsHaveAStatusOfPendingConfirmation_AndTodayIsTheSameDateAsTheItineraryDate()
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var requestorPhoneNumbers = new List<string> { "111-111-1111" };

            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.PendingConfirmation, Phone = requestorPhoneNumbers[0] };
            var itinerary = new Itinerary { Id = 1, Date = dateTimeUtcNow.Date };

            var requestIds = new List<Guid> { request.RequestId };
            var smsSender = new Mock<ISmsSender>();

            Context.Requests.Add(request);
            Context.Itineraries.Add(itinerary);
            Context.SaveChanges();

            var sut = new SendRequestConfirmationMessagesTheDayOfAnItineraryDate(Context, smsSender.Object, Mock.Of<IMediator>()) { DateTimeUtcNow = () => dateTimeUtcNow.Date };
            sut.SendSms(requestIds, itinerary.Id);

            smsSender.Verify(x => x.SendSmsAsync(requestorPhoneNumbers, "sorry you couldn't make it, we will reschedule."));
        }
    }
}