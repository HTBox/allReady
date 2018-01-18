using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.UnlinkedRequests
{
    public class AddRequestsToEventCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnsFalseIfNoEventsReturnedFromContext()
        {
            var sut = new AddRequestsToEventCommandHandler(Context);

            var events = new[]
            {
                new Event() {Id = 123}
            };

            var context = Context;
            context.Events.AddRange(events);
            context.SaveChanges();

            var succeded = await sut.Handle(new AddRequestsToEventCommand() {EventId = 124});

            Assert.False(succeded);
        }

        [Fact]
        public async Task ReturnsFalseIfNoRequestsToUpdateAreFound()
        {
            var expectedEventId = 123;
            var sut = new AddRequestsToEventCommandHandler(Context);

            var events = new[]
            {
                new Event() {Id = expectedEventId}
            };
            var requests = new[]
            {
                new Request() {RequestId = Guid.NewGuid()}
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.Events.AddRange(events);
            context.SaveChanges();

            var succeded = await sut.Handle(new AddRequestsToEventCommand()
            {
                EventId = expectedEventId,
                SelectedRequestIds = new List<Guid>() {Guid.NewGuid()}
            });

            Assert.False(succeded);
        }

        [Fact]
        public async Task UpdatesExpectedRequestsWithSelectedEventId_WhenRequestsFound()
        {
            const int expectedEventId = 123;
            var sut = new AddRequestsToEventCommandHandler(Context);
            var requestId1 = Guid.NewGuid();
            var requestId2 = Guid.NewGuid();
            var requestId3 = Guid.NewGuid();

            var events = new[]
            {
                new Event() {Id = expectedEventId}
            };
            var requests = new[]
            {
                new Request() {RequestId = requestId1},
                new Request() {RequestId = requestId2},
                new Request() {RequestId = requestId3}
            };

            var context = Context;
            context.Requests.AddRange(requests);
            context.Events.AddRange(events);
            context.SaveChanges();

            var succeded = await sut.Handle(new AddRequestsToEventCommand()
            {
                EventId = expectedEventId,
                SelectedRequestIds = new List<Guid>() {requestId1, requestId2, requestId3}
            });

            Assert.True(succeded);
            Assert.Equal(3, Context.Requests.Count());
            Assert.Equal(Context.Requests.Select(x => x.EventId).ToList(),
                new List<int?>() {expectedEventId, expectedEventId, expectedEventId});
            Assert.Equal(Context.Requests.Select(x => x.RequestId).ToList(),
                new List<Guid>() {requestId1, requestId2, requestId3});
        }
    }
}