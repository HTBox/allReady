using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class UpdateEventImageUrlHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task UpdateTheEventsImageUrl()
        {
            const int eventId = 1;

            Context.Events.Add(new Event { Id = eventId });
            await Context.SaveChangesAsync();

            var message = new UpdateEventImageUrl { EventId = eventId, ImageUrl = "ImageUrl" };

            var sut = new UpdateEventImageUrlHandler(Context);
            await sut.Handle(message);

            var result = Context.Events.Single(x => x.Id == eventId);

            Assert.Equal(result.ImageUrl, message.ImageUrl);
        }
    }
}