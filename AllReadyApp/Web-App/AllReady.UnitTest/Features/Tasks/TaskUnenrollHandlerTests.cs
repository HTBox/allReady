using AllReady.Features.Tasks;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class TaskUnenrollHandlerTests : InMemoryContextTest
    {
        [Fact(Skip = "RTM Broken Tests")]
        public async Task Result_ShouldBe_Success_IfTaskSignupExists()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskUnenrollCommand { TaskId = 1, UserId = "abc" };

            var sut = new TaskUnenrollHandlerAsync(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal("success", result.Status);
            Assert.NotNull(result.Task);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task Result_ShouldBe_Failure_IfTaskIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskUnenrollCommand { TaskId = 100, UserId = "abc" };

            var sut = new TaskUnenrollHandlerAsync(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal("failure", result.Status);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task TaskSignUp_ShouldBe_Deleted()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskUnenrollCommand { TaskId = 1, UserId = "abc" };

            var sut = new TaskUnenrollHandlerAsync(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(0, Context.TaskSignups.Count());
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task EventSignUp_ShouldBe_Deleted_IfLastSignupForTheUser()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskUnenrollCommand { TaskId = 1, UserId = "abc" };

            var sut = new TaskUnenrollHandlerAsync(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(0, Context.EventSignup.Count());
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task EventSignUp_ShouldNotBe_Deleted_IfNotLastSignupForTheUser()
        {
            var secondTask = new AllReadyTask { Id = 2, Name = "Some Task 2", EndDateTime = DateTime.UtcNow.AddDays(100), Event = Context.Events.First() };
            Context.Tasks.Add(secondTask);

            Context.TaskSignups.Add(new TaskSignup { Task = secondTask, User = Context.Users.First() });
            Context.SaveChanges();

            var mockMediator = new Mock<IMediator>();
            var message = new TaskUnenrollCommand { TaskId = 1, UserId = "abc" };
            
            var sut = new TaskUnenrollHandlerAsync(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(1, Context.TaskSignups.Count());
            Assert.Equal(1, Context.EventSignup.Count());
        }

        protected override void LoadTestData()
        {
            var user = new ApplicationUser { Id = "abc" };
            Context.Users.Add(user);

            var campaignEvent = new Models.Event { Id = 1, Name = "Some Event" };
            Context.Events.Add(campaignEvent);

            var task = new AllReadyTask { Id = 1, Name = "Some Task", EndDateTime = DateTime.UtcNow.AddDays(100), Event = campaignEvent };
            Context.Tasks.Add(task);

            Context.EventSignup.Add(new EventSignup { Event = campaignEvent, User = user });
            Context.TaskSignups.Add(new TaskSignup { Task = task, User = user });
        
            Context.SaveChanges();
        }
    }
}
