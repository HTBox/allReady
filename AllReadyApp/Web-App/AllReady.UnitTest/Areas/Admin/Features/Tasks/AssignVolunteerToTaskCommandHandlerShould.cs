using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerToTaskCommandHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly AssignVolunteerToTaskCommandHandler sut;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public AssignVolunteerToTaskCommandHandlerShould()
        {
            mediator = new Mock<IMediator>();
            _emailSender = new Mock<IEmailSender>();
            sut = new AssignVolunteerToTaskCommandHandler(Context, mediator.Object, _emailSender.Object);
        }

        [Fact]
        public async Task DoNotAssignUsersAlreadyAssigned()
        {
            var user = new ApplicationUser() {Id = "Answer42"};
            Context.Users.Add(user);
            Context.VolunteerTasks.Add(new VolunteerTask() { Id = 20 });
            Context.VolunteerTaskSignups.Add(new VolunteerTaskSignup() {VolunteerTaskId = 20, User = user});
            await Context.SaveChangesAsync();

            var message = new AssignVolunteerToTaskCommand() {UserId = "Answer42", VolunteerTaskId = 20};
            await sut.Handle(message);

            var count = Context.VolunteerTaskSignups.Count(x => x.User.Id == "Answer42");
            count.ShouldBe(1);
        }

        [Fact]
        public async Task DoNotEmailAlreadyAssignedUsers()
        {
            var user = new ApplicationUser() { Id = "Answer42" };
            Context.Users.Add(user);
            Context.VolunteerTasks.Add(new VolunteerTask() { Id = 20 });
            Context.VolunteerTaskSignups.Add(new VolunteerTaskSignup() { VolunteerTaskId = 20, User = user });
            await Context.SaveChangesAsync();

            var message = new AssignVolunteerToTaskCommand() { UserId = "Answer42", VolunteerTaskId = 20, NotifyUser = true};
            await sut.Handle(message);

            _emailSender.Verify(x=>x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AssignNewUsers()
        {
            var user = new ApplicationUser() { Id = "Answer41PlusOne" };
            Context.Users.Add(user);
            Context.VolunteerTasks.Add(new VolunteerTask() { Id = 20 });
            await Context.SaveChangesAsync();

            var message = new AssignVolunteerToTaskCommand() { UserId = "Answer41PlusOne", VolunteerTaskId = 20 };
            await sut.Handle(message);

            var count = Context.VolunteerTaskSignups.Count(x => x.User.Id == "Answer41PlusOne");
            count.ShouldBe(1);
        }

        [Fact]
        public async Task NewUserShouldGetNotified()
        {
            var user = new ApplicationUser() { Id = "Answer40PlusDeuce" };
            Context.Users.Add(user);
            Context.VolunteerTasks.Add(new VolunteerTask() { Id = 20 });
            await Context.SaveChangesAsync();

            var message = new AssignVolunteerToTaskCommand() { UserId = "Answer40PlusDeuce", VolunteerTaskId = 20, NotifyUser = true};
            await sut.Handle(message);

            _emailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task DoNotNotifyNewUsersWhenDisabled()
        {
            var user = new ApplicationUser() { Id = "Answer40PlusDeuce" };
            Context.Users.Add(user);
            Context.VolunteerTasks.Add(new VolunteerTask() { Id = 20 });
            await Context.SaveChangesAsync();

            var message = new AssignVolunteerToTaskCommand() { UserId = "Answer40PlusDeuce", VolunteerTaskId = 20, NotifyUser = false };
            await sut.Handle(message);

            _emailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

    }
}
