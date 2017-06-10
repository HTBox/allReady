using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;
using AllReady.Services;

using Xunit;
using Moq;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CreateNewTaskSuccessfully()
        {
            var @event = new Event { Id = 1, IsLimitVolunteers = true, IsAllowWaitList = false };
            var organization = new Organization { Id = 2 };

            var message = new EditVolunteerTaskCommand
            {
                VolunteerTask = new EditViewModel
                {
                    Name = "TaskName",
                    Description = "TaskDescription",
                    EventId = @event.Id,
                    OrganizationId = organization.Id,
                    TimeZoneId = "Central Standard Time",
                    NumberOfVolunteersRequired = 5,
                    RequiredSkills = new List<VolunteerTaskSkill> { new VolunteerTaskSkill { SkillId = 3, Skill = new Skill { Id = 3, Name = "SkillName", Description = "SkillDescription" } } }
                }
            };

            Context.Database.EnsureDeleted();
            Context.Events.Add(@event);
            Context.Organizations.Add(organization);
            Context.SaveChanges();

            var mockAttachmentService = new Mock<IAttachmentService>();
            var sut = new EditVolunteerTaskCommandHandler(Context, mockAttachmentService.Object);
            var volunteerTaskId = await sut.Handle(message);
            var result = Context.VolunteerTasks.Single(x => x.Id == volunteerTaskId);

            Assert.True(volunteerTaskId > 0);
            Assert.Equal(result.Name, message.VolunteerTask.Name);
            Assert.Equal(result.Description, message.VolunteerTask.Description);
            Assert.Equal(result.Event, @event);
            Assert.Equal(result.Organization, organization);
            Assert.Equal(result.NumberOfVolunteersRequired, message.VolunteerTask.NumberOfVolunteersRequired);
            Assert.Equal(result.IsLimitVolunteers, @event.IsLimitVolunteers);
            Assert.Equal(result.IsAllowWaitList, @event.IsAllowWaitList);
            Assert.Equal(result.RequiredSkills, message.VolunteerTask.RequiredSkills);
        }

        [Fact]
        public async Task UpdateExistingTaskSuccessfully()
        {
            var @event = new Event { Id = 3 };
            var organization = new Organization { Id = 4 };
            var volunteerTask = new VolunteerTask
            {
                Id = 2,
                Name = "TaskName",
                Description = "TaskDescription",
                Event = @event,
                Organization = organization,
                NumberOfVolunteersRequired = 5,
                RequiredSkills = new List<VolunteerTaskSkill> { new VolunteerTaskSkill { SkillId = 5, Skill = new Skill { Id = 5, Name = "SkillName", Description = "SkillDescription" } } }
            };

            Context.Database.EnsureDeleted();
            Context.Events.Add(@event);
            Context.Organizations.Add(organization);
            Context.VolunteerTasks.Add(volunteerTask);
            Context.SaveChanges();

            var message = new EditVolunteerTaskCommand
            {
                VolunteerTask = new EditViewModel
                {
                    Id = volunteerTask.Id,
                    Name = "TaskNameUpdated",
                    Description = "TaskDescriptionUpdated",
                    EventId = @event.Id,
                    OrganizationId = organization.Id,
                    TimeZoneId = "Central Standard Time",
                    StartDateTime = DateTimeOffset.Now.AddDays(1),
                    EndDateTime = DateTimeOffset.Now.AddDays(2),
                    NumberOfVolunteersRequired = 6,
                    RequiredSkills = new List<VolunteerTaskSkill> { new VolunteerTaskSkill { SkillId = 6, Skill = new Skill { Id = 6, Name = "SkillNameOnMessage", Description = "SkillDescriptionOnMessage" } } }
                }
            };

            var mockAttachmentService = new Mock<IAttachmentService>();
            var sut = new EditVolunteerTaskCommandHandler(Context, mockAttachmentService.Object);
            var volunteerTaskId = await sut.Handle(message);
            var result = Context.VolunteerTasks.Single(x => x.Id == volunteerTaskId);

            //can't test start and end date as they're tied to static classes
            Assert.Equal(volunteerTaskId, message.VolunteerTask.Id);
            Assert.Equal(result.Name, message.VolunteerTask.Name);
            Assert.Equal(result.Description, message.VolunteerTask.Description);
            Assert.Equal(result.Event, @event);
            Assert.Equal(result.Organization, organization);
            Assert.Equal(result.NumberOfVolunteersRequired, message.VolunteerTask.NumberOfVolunteersRequired);
            Assert.Equal(result.IsLimitVolunteers, @event.IsLimitVolunteers);
            Assert.Equal(result.IsAllowWaitList, @event.IsAllowWaitList);
            Assert.Equal(result.RequiredSkills, message.VolunteerTask.RequiredSkills);
        }
    }
}