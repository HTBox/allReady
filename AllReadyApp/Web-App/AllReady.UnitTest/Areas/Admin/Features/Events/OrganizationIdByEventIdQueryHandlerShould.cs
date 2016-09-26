using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class OrganizationIdByEventIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var @event = new Event { Campaign = new Campaign { ManagingOrganization = new Organization { Id = 1 }}};
            Context.Events.Add(@event);
            await Context.SaveChangesAsync();

            var message = new OrganizationIdByEventIdQuery { EventId = @event.Id };

            var sut = new OrganizationIdByEventIdQueryHandler(Context);
            var result = await sut.Handle(message);

            Assert.Equal(result, @event.Campaign.ManagingOrganization.Id);
        }
    }
}
