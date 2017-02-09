using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskQueryHandlerShould : InMemoryContextTest
    {
        private readonly EditVolunteerTaskQuery message;
        private readonly EditVolunteerTaskQueryHandler sut;
        private readonly VolunteerTask task;

        public EditVolunteerTaskQueryHandlerShould()
        {
            task = new VolunteerTask
            {
                Id = 1,
                Name = "Taskname",
                Description = "Description",
                StartDateTime = DateTimeOffset.Now,
                EndDateTime = DateTimeOffset.Now,
                NumberOfVolunteersRequired = 5,
                RequiredSkills = new List<VolunteerTaskSkill> { new VolunteerTaskSkill { SkillId = 2, Skill = new Skill(), VolunteerTaskId = 1 } },
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

            message = new EditVolunteerTaskQuery { VolunteerTaskId = task.Id };
            sut = new EditVolunteerTaskQueryHandler(Context);

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