using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class DetailsQueryHandlerShould : InMemoryContextTest
    {
        private readonly VolunteerTask task;
        private readonly DetailsQuery message;
        private readonly DetailsQueryHandler sut;

        public DetailsQueryHandlerShould()
        {
            task = new VolunteerTask
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
                    Campaign = new Campaign { Id = 3, Name = "CampaignName", TimeZoneId = "Central Standard Time" },
                    TimeZoneId = "Central Standard Time"
                },
                RequiredSkills = new List<VolunteerTaskSkill> { new VolunteerTaskSkill { SkillId = 4, VolunteerTaskId = 1 } },
                AssignedVolunteers = new List<VolunteerTaskSignup>
                {
                    new VolunteerTaskSignup
                    {
                        User = new ApplicationUser
                        {
                            Id = "UserId",
                            UserName = "UserName",
                            FirstName = "FirstName",
                            LastName = "LastName",
                            PhoneNumber = "PhoneNumber",
                            AssociatedSkills = new List<UserSkill> { new UserSkill { Skill = new Skill { Name = "Skill", ParentSkill = new Skill { Name = "Parent skill" } } } }
                        }
                    }
                }
            };

            Context.VolunteerTasks.Add(task);
            Context.SaveChanges();

            message = new DetailsQuery { VolunteerTaskId = task.Id };
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
            Assert.Equal(result.TimeZoneId, task.Event.TimeZoneId);
            //Assert.Equal(result.RequiredSkills, task.RequiredSkills);
            Assert.Equal(result.AssignedVolunteers.Count, task.AssignedVolunteers.Count);
            for (var i = 0; i < result.AssignedVolunteers.Count; ++i)
            {
                var resultVolunteer = result.AssignedVolunteers[i];
                var expectedVolunteer = task.AssignedVolunteers[i];
                Assert.Equal(resultVolunteer.UserId, expectedVolunteer.User.Id);
                Assert.Equal(resultVolunteer.UserName, expectedVolunteer.User.UserName);
                Assert.True(resultVolunteer.HasVolunteered);
                Assert.Equal(resultVolunteer.Name, expectedVolunteer.User.Name);
                Assert.Equal(resultVolunteer.PhoneNumber, expectedVolunteer.User.PhoneNumber);
                Assert.Equal(resultVolunteer.AssociatedSkills.Count, expectedVolunteer.User.AssociatedSkills.Count);
                for (var j = 0; j < resultVolunteer.AssociatedSkills.Count; ++j)
                {
                    Assert.Equal(resultVolunteer.AssociatedSkills[j].Skill.HierarchicalName, resultVolunteer.AssociatedSkills[j].Skill.HierarchicalName);
                }
            }
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