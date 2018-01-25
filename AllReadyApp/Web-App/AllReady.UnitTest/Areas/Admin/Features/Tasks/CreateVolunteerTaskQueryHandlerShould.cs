using System;
using System.Threading.Tasks;

using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class CreateVolunteerTaskQueryHandlerShould : InMemoryContextTest
    {
        private readonly Event @event;
        private readonly CreateVolunteerTaskQuery message;
        private readonly CreateVolunteerTaskQueryHandler sut;

        public CreateVolunteerTaskQueryHandlerShould()
        {
            @event = new Event
            {
                Id = 1,
                Name = "EventName",
                StartDateTime = DateTimeOffset.Now,
                EndDateTime = DateTimeOffset.Now,
                Campaign = new Campaign { StartDateTime = DateTimeOffset.Now, EndDateTime = DateTimeOffset.Now, Name = "CampaignName", ManagingOrganizationId = 2, TimeZoneId = "Central Standard Time" },
                TimeZoneId = "Central Standard Time"
            };

            Context.Add(@event);
            Context.SaveChanges();

            message = new CreateVolunteerTaskQuery { EventId = @event.Id };
            sut = new CreateVolunteerTaskQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);

            Assert.Equal(@event.Id, result.EventId);
            Assert.Equal(@event.Name, result.EventName);
            Assert.Equal(@event.StartDateTime, result.EventStartDate);
            Assert.Equal(@event.EndDateTime, result.EventEndDate);
            Assert.Equal(@event.StartDateTime, result.StartDateTime);
            Assert.Equal(@event.EndDateTime, result.EndDateTime);           
            Assert.Equal(@event.CampaignId, result.CampaignId);
            Assert.Equal(@event.Campaign.Name, result.CampaignName);
            Assert.Equal(@event.Campaign.ManagingOrganizationId, result.OrganizationId);
            Assert.Equal(@event.TimeZoneId, result.TimeZoneId);
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var result = await sut.Handle(message);
            Assert.IsType<EditViewModel>(result);
        }
    }
}
