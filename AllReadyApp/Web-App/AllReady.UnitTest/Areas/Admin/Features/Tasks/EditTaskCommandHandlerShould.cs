using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using Xunit;
using System.Linq;
using AllReady.Providers;
using Moq;
using AllReady.Services;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CreateNewTaskSuccessfully()
        {
            var @event = new Event { Id = 1, IsLimitVolunteers = true, IsAllowWaitList = false };
            var organization = new Organization { Id = 2 };

            var message = new EditTaskCommand
            {
                Task = new EditViewModel
                {
                    Name = "TaskName",
                    Description = "TaskDescription",
                    EventId = @event.Id,
                    OrganizationId = organization.Id,
                    TimeZoneId = "Central Standard Time",
                    NumberOfVolunteersRequired = 5,
                    RequiredSkills = new List<TaskSkill> { new TaskSkill { SkillId = 3, Skill = new Skill { Id = 3, Name = "SkillName", Description = "SkillDescription" } } }
                }
            };

            Context.Database.EnsureDeleted();
            Context.Events.Add(@event);
            Context.Organizations.Add(organization);
            Context.SaveChanges();

            var mockAttachmentService = new Mock<IAttachmentService>();
            var sut = new EditTaskCommandHandler(Context, mockAttachmentService.Object);
            var taskId = await sut.Handle(message);
            var result = Context.Tasks.Single(x => x.Id == taskId);

            Assert.True(taskId > 0);
            Assert.Equal(result.Name, message.Task.Name);
            Assert.Equal(result.Description, message.Task.Description);
            Assert.Equal(result.Event, @event);
            Assert.Equal(result.Organization, organization);
            Assert.Equal(result.NumberOfVolunteersRequired, message.Task.NumberOfVolunteersRequired);
            Assert.Equal(result.IsLimitVolunteers, @event.IsLimitVolunteers);
            Assert.Equal(result.IsAllowWaitList, @event.IsAllowWaitList);
            Assert.Equal(result.RequiredSkills, message.Task.RequiredSkills);
        }

        [Fact]
        public async Task UpdateExistingTaskSuccessfully()
        {
            var @event = new Event { Id = 3 };
            var organization = new Organization { Id = 4 };
            var @task = new AllReadyTask
            {
                Id = 2,
                Name = "TaskName",
                Description = "TaskDescription",
                Event = @event,
                Organization = organization,
                NumberOfVolunteersRequired = 5,
                RequiredSkills = new List<TaskSkill> { new TaskSkill { SkillId = 5, Skill = new Skill { Id = 5, Name = "SkillName", Description = "SkillDescription" } } }
            };

            Context.Database.EnsureDeleted();
            Context.Events.Add(@event);
            Context.Organizations.Add(organization);
            Context.Tasks.Add(@task);
            Context.SaveChanges();

            var message = new EditTaskCommand
            {
                Task = new EditViewModel
                {
                    Id = @task.Id,
                    Name = "TaskNameUpdated",
                    Description = "TaskDescriptionUpdated",
                    EventId = @event.Id,
                    OrganizationId = organization.Id,
                    TimeZoneId = "Central Standard Time",
                    StartDateTime = DateTimeOffset.Now.AddDays(1),
                    EndDateTime = DateTimeOffset.Now.AddDays(2),
                    NumberOfVolunteersRequired = 6,
                    RequiredSkills = new List<TaskSkill> { new TaskSkill { SkillId = 6, Skill = new Skill { Id = 6, Name = "SkillNameOnMessage", Description = "SkillDescriptionOnMessage" } } }
                }
            };

            var mockAttachmentService = new Mock<IAttachmentService>();
            var sut = new EditTaskCommandHandler(Context, mockAttachmentService.Object);
            var taskId = await sut.Handle(message);
            var result = Context.Tasks.Single(x => x.Id == taskId);

            //can't test start and end date as they're tied to static classes
            Assert.Equal(taskId, message.Task.Id);
            Assert.Equal(result.Name, message.Task.Name);
            Assert.Equal(result.Description, message.Task.Description);
            Assert.Equal(result.Event, @event);
            Assert.Equal(result.Organization, organization);
            Assert.Equal(result.NumberOfVolunteersRequired, message.Task.NumberOfVolunteersRequired);
            Assert.Equal(result.IsLimitVolunteers, @event.IsLimitVolunteers);
            Assert.Equal(result.IsAllowWaitList, @event.IsAllowWaitList);
            Assert.Equal(result.RequiredSkills, message.Task.RequiredSkills);
        }
    }
}