using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class DeleteQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnCorrectData()
        {
            var @event = new Event { Id = 1, Campaign = new Campaign { Id = 1, Name = "CampaignName", ManagingOrganization = new Organization { Id = 1 }}};
            Context.Events.Add(@event);
            Context.SaveChanges();

            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { EventId = @event.Id });

            Assert.Equal(result.Id, @event.Id);
            Assert.Equal(result.Name, @event.Name);
            Assert.Equal(result.CampaignId, @event.Campaign.Id);
            Assert.Equal(result.CampaignName, @event.Campaign.Name);
            Assert.Equal(result.OrganizationId, @event.Campaign.ManagingOrganization.Id);
            Assert.Equal(result.StartDateTime, @event.StartDateTime);
            Assert.Equal(result.EndDateTime, @event.EndDateTime);
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            const int eventId = 1;

            Context.Events.Add(new Event
            {
                Id = eventId,
                Campaign = new Campaign { ManagingOrganization = new Organization() }
            });
            Context.SaveChanges();

            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { EventId = eventId });

            Assert.IsType<DeleteViewModel>(result);
        }
    }
}
