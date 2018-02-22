using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
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
                    StartDateTime = DateTimeOffset.UtcNow.AddDays(-2),
                    EndDateTime = DateTimeOffset.Now.AddDays(-1),
                    Campaign = new Campaign
                    {
                        StartDateTime = DateTimeOffset.Now.AddDays(-5),
                        EndDateTime = DateTimeOffset.Now.AddDays(-3),
                        Name = "CampaignName",
                        ManagingOrganizationId = 5,
                        TimeZoneId = "Central Standard Time"
                    },
                    TimeZoneId = "Central Standard Time"
                }
            };

            Context.VolunteerTasks.Add(task);
            Context.SaveChanges();

            message = new EditVolunteerTaskQuery { VolunteerTaskId = task.Id };
            sut = new EditVolunteerTaskQueryHandler(Context);

        }

        [Fact]
        public async Task ReturnsCorrectData()
        {
            var result = await sut.Handle(message);

            Assert.Equal(task.Id, result.Id);
            Assert.Equal(task.Name, result.Name);
            Assert.Equal(task.Description, result.Description);
            Assert.Equal(task.Event.StartDateTime, result.EventStartDate);
            Assert.Equal(task.Event.EndDateTime, result.EventEndDate);
            Assert.Equal(task.NumberOfVolunteersRequired, result.NumberOfVolunteersRequired);
            Assert.Equal(task.Event.Id, result.EventId);
            Assert.Equal(task.Event.Name, result.EventName);
            Assert.Equal(task.StartDateTime, result.StartDateTime);
            Assert.Equal(task.EndDateTime, result.EndDateTime);
            Assert.Equal(task.Event.CampaignId, result.CampaignId);
            Assert.Equal(task.Event.Campaign.Name, result.CampaignName);
            Assert.Equal(task.Event.Campaign.ManagingOrganizationId, result.OrganizationId);
            Assert.Equal(task.Event.TimeZoneId, result.TimeZoneId);
        }

        [Fact]
        public async Task ReturnsCorrectViewModel()
        {
            var result = await sut.Handle(message);
            Assert.IsType<EditViewModel>(result);
        }
    }
}
