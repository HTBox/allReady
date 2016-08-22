using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class UpdateEventHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task RemoveSkillsNoLongerAssociatedWithTheEvent()
        {
            var eventToSeed = new Event { Id = 1, RequiredSkills = null };
            var eventSkill = new EventSkill { EventId = eventToSeed.Id, Event = eventToSeed };

            Context.Database.EnsureDeleted();
            Context.Add(eventToSeed);
            Context.Add(eventSkill);
            Context.SaveChanges();

            var eventToUpdate = Context.Events.Single(x => x.Id == eventToSeed.Id);
            eventToUpdate.RequiredSkills = null;

            var message = new UpdateEvent { Event = eventToUpdate };
             
            var sut = new UpdateEventHandler(Context);
            await sut.Handle(message);

            var eventSkillResult = Context.EventSkills.SingleOrDefault(x => x.EventId == eventToSeed.Id);
            Assert.Null(eventSkillResult);
        }

        [Fact]
        public async Task UpdateEvent()
        {
            var eventToSeed = new Event { Id = 1, Name = "EventName" };

            Context.Database.EnsureDeleted();
            Context.Add(eventToSeed);
            Context.SaveChanges();

            var eventToUpdate = Context.Events.Single(x => x.Id == eventToSeed.Id);
            eventToUpdate.Name = "EventNameUpdated";

            var message = new UpdateEvent { Event = eventToUpdate };

            var sut = new UpdateEventHandler(Context);
            await sut.Handle(message);

            var result = Context.Events.Single(x => x.Id == eventToSeed.Id);
            Assert.Equal(result.Name, message.Event.Name);
        }
    }
}