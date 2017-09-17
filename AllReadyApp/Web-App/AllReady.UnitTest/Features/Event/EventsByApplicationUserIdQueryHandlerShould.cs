using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByApplicationUserIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly EventsByApplicationUserIdQuery message;
        private readonly Event @event;
        private readonly EventsByApplicationUserIdQueryHandler sut;


        public EventsByApplicationUserIdQueryHandlerShould()
        {
            message = new EventsByApplicationUserIdQuery { ApplicationUserId = Guid.NewGuid().ToString() };
            @event = new Event { Organizer = new ApplicationUser {Id = message.ApplicationUserId } };


            Context.Add(@event);
            Context.SaveChanges();

            sut = new EventsByApplicationUserIdQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectAmount()
        {
            var result = await sut.Handle(message);
            Assert.Single(result);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);
            Assert.Same(@event, result.First());
        }

        [Fact]
        public async Task ReturnCorrectType()
        {
            var result = await sut.Handle(message);
            Assert.IsType<Event>(result.First());
        }
    }
}