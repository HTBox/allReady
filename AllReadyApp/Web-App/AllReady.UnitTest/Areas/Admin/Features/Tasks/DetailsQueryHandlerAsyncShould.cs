using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DetailsQueryHandlerAsyncShould : InMemoryContextTest
    {
        private readonly AllReadyTask task;
        private readonly DetailsQuery message;
        private readonly DetailsQueryHandler sut;

        public DetailsQueryHandlerAsyncShould()
        {
            task = new AllReadyTask
            {
                Id = 1,
                Name = "TaskName",
                Description = "TaskDescription",
                StartDateTime = DateTimeOffset.Now,
                EndDateTime = DateTimeOffset.Now,
                NumberOfVolunteersRequired = 5,
                Event = new Event
                {
                    Id = 2,
                    Name = "EventName",
                    CampaignId = 3,
                    Campaign = new Campaign { Id = 3, Name = "CampaignName", TimeZoneId = "Central Standard Time" }
                },
                RequiredSkills = new List<TaskSkill> { new TaskSkill { SkillId = 4, TaskId = 1 } },
                AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User = new ApplicationUser { Id = "UserId", UserName = "UserName" } } }
            };

            Context.Tasks.Add(task);
            Context.SaveChanges();

            message = new DetailsQuery { TaskId = task.Id };
            sut = new DetailsQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);

            Assert.Equal(result.Id, task.Id);
            Assert.Equal(result.Name, task.Name);
            Assert.Equal(result.Description, task.Description);
            Assert.Equal(result.NumberOfVolunteersRequired, task.NumberOfVolunteersRequired);
            Assert.Equal(result.EventId, task.Event.Id);
            Assert.Equal(result.EventName, task.Event.Name);
            Assert.Equal(result.CampaignId, task.Event.CampaignId);
            Assert.Equal(result.CampaignName, task.Event.Campaign.Name);
            Assert.Equal(result.TimeZoneId, task.Event.Campaign.TimeZoneId);
            //Assert.Equal(result.RequiredSkills, task.RequiredSkills);
            //Assert.Equal(result.AssignedVolunteers, task.AssignedVolunteers.Select(x => new VolunteerViewModel { UserId = x.User.Id, UserName = x.User.UserName, HasVolunteered = true }).ToList());
            //Assert.Equal(result.AllVolunteers, task.Event.UsersSignedUp.Select(x => new VolunteerViewModel { UserId = x.User.Id, UserName = x.User.UserName, HasVolunteered = false }).ToList());
        }

        [Fact]
        public async Task ReturnCorrectViewModel()
        {
            var result = await sut.Handle(message);
            Assert.IsType<DetailsViewModel>(result);
        }
    }
}