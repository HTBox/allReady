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

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerTaskCommandHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly AssignVolunteerTaskCommandHandler sut;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public AssignVolunteerTaskCommandHandlerShould()
        {
            mediator = new Mock<IMediator>();
            sut = new AssignVolunteerTaskCommandHandler(Context, mediator.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
        }

        [Fact]
        public async Task AssignsVolunteersToTask()
        {
            var newVolunteer = new ApplicationUser { Id = "user1", Email = "user1@abc.com", PhoneNumber = "1234"};
            var volunteerTask = new VolunteerTask { Id = 1 };
            Context.Add(newVolunteer);
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new AssignVolunteerTaskCommand { VolunteerTaskId = volunteerTask.Id, UserIds = new List<string> { newVolunteer.Id } };
            await sut.Handle(message);

            var volunteerTaskSignup = Context.VolunteerTasks.Single(x => x.Id == volunteerTask.Id).AssignedVolunteers.Single();
            Assert.Equal(volunteerTaskSignup.User.Id, newVolunteer.Id);
            Assert.Equal(VolunteerTaskStatus.Assigned, volunteerTaskSignup.Status);
            Assert.Equal(volunteerTaskSignup.StatusDateTimeUtc, dateTimeUtcNow);
        }

        [Fact]
        public async Task RemoveUsersThatAreNotInTheCurrentSignUpList()
        {
            var volunteerTask = new VolunteerTask { Id = 1 };
            Context.Add(new ApplicationUser {Id = "user2"});
            var previouslySignedupUser = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234"
            };
            volunteerTask.AssignedVolunteers = new List<VolunteerTaskSignup> { new VolunteerTaskSignup { User =  previouslySignedupUser} };
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new AssignVolunteerTaskCommand { VolunteerTaskId = volunteerTask.Id, UserIds = new List<string> { "user2" } };
            await sut.Handle(message);

            Assert.Contains(Context.VolunteerTasks.Single(x => x.Id == volunteerTask.Id).AssignedVolunteers, x => x.User.Id != previouslySignedupUser.Id);
        }

        [Fact]
        public async Task PublishTaskAssignedToVolunteersNotification()
        {
            var volunteerTask = new VolunteerTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            Context.Add(volunteer);
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new AssignVolunteerTaskCommand { VolunteerTaskId = volunteerTask.Id, UserIds = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskAssignedToVolunteersNotification>(notification =>
                   notification.VolunteerTaskId == message.VolunteerTaskId &&
                   notification.NewlyAssignedVolunteers.Contains(volunteer.Id)
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotPublishTaskAssignedNotificationToVolunteersPreviouslySignedUp()
        {
            var volunteerTask = new VolunteerTask { Id = 1 };
            var previouslySignedupUser = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            Context.Add(previouslySignedupUser);
            volunteerTask.AssignedVolunteers = new List<VolunteerTaskSignup> { new VolunteerTaskSignup { User =  previouslySignedupUser } };
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new AssignVolunteerTaskCommand { VolunteerTaskId = volunteerTask.Id, UserIds = new List<string> { previouslySignedupUser.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskAssignedToVolunteersNotification>(notification => !notification.NewlyAssignedVolunteers.Contains(previouslySignedupUser.Id) )), Times.Once);
        }
    }
}
