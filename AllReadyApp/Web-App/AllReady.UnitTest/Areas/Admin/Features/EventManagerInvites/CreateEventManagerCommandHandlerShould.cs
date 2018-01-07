using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerCommandHandlerShould : InMemoryContextTest
    {
        private string _userId = Guid.NewGuid().ToString();
        private int _eventId = 10;

        [Fact]
        public async Task AddEventManager()
        {
            var handler = new CreateEventManagerCommandHandler(Context);
            await handler.Handle(new CreateEventManagerCommand
            {
                EventId = _eventId,
                UserId = _userId
            });

            Context.EventManagers.Count().ShouldBe(1);
            Context.EventManagers.First().EventId.ShouldBe(_eventId);
            Context.EventManagers.First().UserId.ShouldBe(_userId);
        }

        public async Task NotAddEventManagerIfAllreadyExist()
        {
            Context.EventManagers.Add(new EventManager
            {
                EventId = _eventId,
                UserId = _userId
            });

            Context.SaveChanges();

            Context.EventManagers.Count().ShouldBe(1);

            var handler = new CreateEventManagerCommandHandler(Context);
            await handler.Handle(new CreateEventManagerCommand
            {
                EventId = _eventId,
                UserId = _userId
            });

            Context.EventManagers.Count().ShouldBe(1);
        }
    }
}
