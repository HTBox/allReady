using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class EditTaskQueryHandlerShould : InMemoryContextTest
    {
        private readonly EditTaskQuery message;
        private readonly EditTaskQueryHandler sut;
        private readonly AllReadyTask task;

        public EditTaskQueryHandlerShould()
        {
            task = new AllReadyTask
            {
                Id = 1,
                Name = "Taskname",
                Description = "Description",
                StartDateTime = DateTimeOffset.Now,
                EndDateTime = DateTimeOffset.Now,
                NumberOfVolunteersRequired = 5,
                RequiredSkills = new List<TaskSkill> { new TaskSkill { SkillId = 2, Skill = new Skill(), TaskId = 1 } },
                Event = new Event
                {
                    Id = 3,
                    Name = "EventName",
                    CampaignId = 4,
                    Campaign = new Campaign
                    {
                        StartDateTime = DateTimeOffset.Now,
                        EndDateTime = DateTimeOffset.Now,
                        Name = "CampaignName",
                        ManagingOrganizationId = 5,
                        TimeZoneId = "Central Standard Time"
                    },
                    TimeZoneId = "Central Standard Time"
                }
            };

            Context.Tasks.Add(task);
            Context.SaveChanges();

            message = new EditTaskQuery { TaskId = task.Id };
            sut = new EditTaskQueryHandler(Context);

        }

        [Fact]
        public async Task ReturnsCorrectData()
        {
            var result = await sut.Handle(message);

            Assert.Equal(result.Id, task.Id);
            Assert.Equal(result.Name, task.Name);
            Assert.Equal(result.Description, task.Description);
            Assert.Equal(result.StartDateTime, task.StartDateTime);
            Assert.Equal(result.EndDateTime, task.EndDateTime);
            Assert.Equal(result.NumberOfVolunteersRequired, task.NumberOfVolunteersRequired);
            Assert.Equal(result.EventId, task.Event.Id);
            Assert.Equal(result.EventName, task.Event.Name);
            Assert.Equal(result.CampaignId, task.Event.CampaignId);
            Assert.Equal(result.EventStartDateTime, task.Event.StartDateTime);
            Assert.Equal(result.EventEndDateTime, task.Event.EndDateTime);
            Assert.Equal(result.CampaignName, task.Event.Campaign.Name);
            Assert.Equal(result.OrganizationId, task.Event.Campaign.ManagingOrganizationId);
            Assert.Equal(result.TimeZoneId, task.Event.TimeZoneId);
        }

        [Fact]
        public async Task ReturnsCorrectViewModel()
        {
            var result = await sut.Handle(message);
            Assert.IsType<EditViewModel>(result);
        }
    }
}