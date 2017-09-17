using System;
using System.Threading.Tasks;

using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DeleteQueryHandlerShould : InMemoryContextTest
    {
        private readonly VolunteerTask volunteerTask;
        private const int VolunteerTaskId = 1;

        public DeleteQueryHandlerShould()
        {
            volunteerTask = new VolunteerTask
            {
                Id = 1,
                Name = "TaskName",
                StartDateTime = new DateTimeOffset().UtcDateTime,
                EndDateTime = new DateTimeOffset().UtcDateTime,
                Event = new Event { Id = 2, Name = "EventName", CampaignId = 3, Campaign = new Campaign { ManagingOrganizationId = 4, Name = "Campaign Name" } }
            };
            Context.VolunteerTasks.Add(volunteerTask);

            var volunteerTaskThatShouldNotBeReturnedFromQuery = new VolunteerTask { Id = 2 };
            Context.VolunteerTasks.Add(volunteerTaskThatShouldNotBeReturnedFromQuery);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { VolunteerTaskId = VolunteerTaskId });

            Assert.Equal(result.Id, volunteerTask.Id);
            Assert.Equal(result.OrganizationId, volunteerTask.Event.Campaign.ManagingOrganizationId);
            Assert.Equal(result.CampaignId, volunteerTask.Event.CampaignId);
            Assert.Equal(result.CampaignName, volunteerTask.Event.Campaign.Name);
            Assert.Equal(result.EventId, volunteerTask.Event.Id);
            Assert.Equal(result.EventName, volunteerTask.Event.Name);
            Assert.Equal(result.Name, volunteerTask.Name);
            Assert.Equal(result.StartDateTime, volunteerTask.StartDateTime);
            Assert.Equal(result.EndDateTime, volunteerTask.EndDateTime);
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { VolunteerTaskId = VolunteerTaskId });

            Assert.IsType<DeleteViewModel>(result);
        }
    }
}