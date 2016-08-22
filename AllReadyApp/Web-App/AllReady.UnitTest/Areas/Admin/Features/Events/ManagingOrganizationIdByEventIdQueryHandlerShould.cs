using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly Event @event;
        private readonly ManagingOrganizationIdByEventIdQuery message;
        private readonly ManagingOrganizationIdByEventIdQueryHandler sut;

        public ManagingOrganizationIdByEventIdQueryHandlerShould()
        {
            @event = new Event { Id = 1, Campaign = new Campaign { ManagingOrganizationId = 2 }};

            Context.Events.Add(@event);
            Context.SaveChanges();

            message = new ManagingOrganizationIdByEventIdQuery { EventId = @event.Id };
            sut = new ManagingOrganizationIdByEventIdQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);
            Assert.Equal(result, @event.Campaign.ManagingOrganizationId);
        }

    }
}