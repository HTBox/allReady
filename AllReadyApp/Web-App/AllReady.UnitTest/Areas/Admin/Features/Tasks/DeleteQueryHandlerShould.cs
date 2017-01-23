using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DeleteQueryHandlerShould : InMemoryContextTest
    {
        private readonly VolunteerTask task;
        private const int TaskId = 1;

        public DeleteQueryHandlerShould()
        {
            task = new VolunteerTask
            {
                Id = 1,
                Name = "TaskName",
                StartDateTime = new DateTimeOffset().UtcDateTime,
                EndDateTime = new DateTimeOffset().UtcDateTime,
                Event = new Event { Id = 2, Name = "EventName", CampaignId = 3, Campaign = new Campaign { ManagingOrganizationId = 4, Name = "Campaign Name" } }
            };
            Context.Tasks.Add(task);

            var taskThatShouldNotBeReturnedFromQuery = new VolunteerTask { Id = 2 };
            Context.Tasks.Add(taskThatShouldNotBeReturnedFromQuery);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { TaskId = TaskId });

            Assert.Equal(result.Id, task.Id);
            Assert.Equal(result.OrganizationId, task.Event.Campaign.ManagingOrganizationId);
            Assert.Equal(result.CampaignId, task.Event.CampaignId);
            Assert.Equal(result.CampaignName, task.Event.Campaign.Name);
            Assert.Equal(result.EventId, task.Event.Id);
            Assert.Equal(result.EventName, task.Event.Name);
            Assert.Equal(result.Name, task.Name);
            Assert.Equal(result.StartDateTime, task.StartDateTime);
            Assert.Equal(result.EndDateTime, task.EndDateTime);
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var sut = new DeleteQueryHandler(Context);
            var result = await sut.Handle(new DeleteQuery { TaskId = TaskId });

            Assert.IsType<DeleteViewModel>(result);
        }
    }
}