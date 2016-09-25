using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class CreateTaskQueryHandlerAsyncShould : InMemoryContextTest
    {
        private readonly Event @event;
        private readonly CreateTaskQuery message;
        private readonly CreateTaskQueryHandler sut;

        public CreateTaskQueryHandlerAsyncShould()
        {
            @event = new Event
            {
                Id = 1,
                Name = "EventName",
                StartDateTime = DateTimeOffset.Now,
                EndDateTime = DateTimeOffset.Now,
                Campaign = new Campaign { StartDateTime = DateTimeOffset.Now, EndDateTime = DateTimeOffset.Now, Name = "CampaignName", ManagingOrganizationId = 2, TimeZoneId = "Central Standard Time" }
            };

            Context.Add(@event);
            Context.SaveChanges();

            message = new CreateTaskQuery { EventId = @event.Id };
            sut = new CreateTaskQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);

            Assert.Equal(result.EventId, @event.Id);
            Assert.Equal(result.EventName, @event.Name);
            Assert.Equal(result.StartDateTime, @event.StartDateTime);
            Assert.Equal(result.EndDateTime, @event.EndDateTime);
            Assert.Equal(result.EventStartDateTime, @event.Campaign.StartDateTime);
            Assert.Equal(result.EventEndDateTime, @event.Campaign.EndDateTime);
            Assert.Equal(result.CampaignId, @event.CampaignId);
            Assert.Equal(result.CampaignName, @event.Campaign.Name);
            Assert.Equal(result.OrganizationId, @event.Campaign.ManagingOrganizationId);
            Assert.Equal(result.TimeZoneId, @event.Campaign.TimeZoneId);
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var result = await sut.Handle(message);
            Assert.IsType<EditViewModel>(result);
        }
    }
}
