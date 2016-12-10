using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;
using System.Linq;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly AssignTaskCommandHandler sut;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public AssignTaskCommandHandlerShould()
        {
            mediator = new Mock<IMediator>();
            sut = new AssignTaskCommandHandler(Context, mediator.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
        }

        [Fact]
        public async Task AssignsVolunteersToTask()
        {
            var newVolunteer = new ApplicationUser { Id = "user1", Email = "user1@abc.com", PhoneNumber = "1234"};
            var theTask = new AllReadyTask { Id = 1 };
            Context.Add(newVolunteer);
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new AssignTaskCommand { TaskId = theTask.Id, UserIds = new List<string> { newVolunteer.Id } };
            await sut.Handle(message);

            var taskSignup = Context.Tasks.Single(x => x.Id == theTask.Id).AssignedVolunteers.Single();
            Assert.Equal(taskSignup.User.Id, newVolunteer.Id);
            Assert.Equal(taskSignup.Status, TaskStatus.Assigned.ToString());
            Assert.Equal(taskSignup.StatusDateTimeUtc, dateTimeUtcNow);
        }

        [Fact]
        public async Task RemoveUsersThatAreNotInTheCurrentSignUpList()
        {
            var theTask = new AllReadyTask { Id = 1 };
            Context.Add(new ApplicationUser {Id = "user2"});
            var previouslySignedupUser = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234"
            };
            theTask.AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User =  previouslySignedupUser} };
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new AssignTaskCommand { TaskId = theTask.Id, UserIds = new List<string> { "user2" } };
            await sut.Handle(message);

            Assert.True(Context.Tasks.Single(x => x.Id == theTask.Id).AssignedVolunteers.Any(x => x.User.Id != previouslySignedupUser.Id ));
        }

        [Fact]
        public async Task PublishTaskAssignedToVolunteersNotification()
        {
            var theTask = new AllReadyTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            Context.Add(volunteer);
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new AssignTaskCommand { TaskId = theTask.Id, UserIds = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.PublishAsync(It.Is<TaskAssignedToVolunteersNotification>(notification =>
                   notification.TaskId == message.TaskId &&
                   notification.NewlyAssignedVolunteers.Contains(volunteer.Id)
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotPublishTaskAssignedNotificationToVolunteersPreviouslySignedUp()
        {
            var theTask = new AllReadyTask { Id = 1 };
            var previouslySignedupUser = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            Context.Add(previouslySignedupUser);
            theTask.AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User =  previouslySignedupUser } };
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new AssignTaskCommand { TaskId = theTask.Id, UserIds = new List<string> { previouslySignedupUser.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.PublishAsync(It.Is<TaskAssignedToVolunteersNotification>(notification => !notification.NewlyAssignedVolunteers.Contains(previouslySignedupUser.Id) )), Times.Once);
        }
    }
}