using AllReady.Features.Tasks;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Shared;
using Xunit;

namespace AllReady.UnitTest.Features.Tasks
{
    public class TaskSignupHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task Result_ShouldBe_ClosedTaskFailure_IfTaskIsClosed()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskSignupCommand { TaskSignupModel = new TaskSignupViewModel { TaskId = 1, EventId = 1, UserId = "abc" } };

            var sut = new TaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskSignupResult.FAILURE_CLOSEDTASK, result.Status);
            Assert.Equal(0, Context.TaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_CampaignNotFound_IfCampaignIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskSignupCommand { TaskSignupModel = new TaskSignupViewModel { TaskId = 1, EventId = 100, UserId = "abc" } };

            var sut = new TaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskSignupResult.FAILURE_EVENTNOTFOUND, result.Status);
            Assert.Equal(0, Context.TaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_TaskNotFound_IfTaskIdDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskSignupCommand { TaskSignupModel = new TaskSignupViewModel { TaskId = 100, EventId = 1, UserId = "abc" } };

            var sut = new TaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskSignupResult.FAILURE_TASKNOTFOUND, result.Status);
            Assert.Equal(0, Context.TaskSignups.Count());
        }

        [Fact]
        public async Task Result_ShouldBe_Success_IfTaskIsNotClosed()
        {
            var mockMediator = new Mock<IMediator>();
            var message = new TaskSignupCommand { TaskSignupModel = new TaskSignupViewModel { TaskId = 2, EventId = 1, UserId = "abc" } };

            var sut = new TaskSignupCommandHandler(mockMediator.Object, Context);
            var result = await sut.Handle(message);

            Assert.Equal(TaskSignupResult.SUCCESS, result.Status);
            Assert.Equal(1, Context.TaskSignups.Count());
        }

        protected override void LoadTestData()
        {
            Context.Users.Add(new ApplicationUser { Id = "abc" });

            var campaignEvent = new Models.Event { Id = 1, Name = "Some Event" };
            Context.Events.Add(campaignEvent);

            Context.Tasks.Add(new AllReadyTask { Id = 1, Name = "Closed Task", EndDateTime = DateTime.UtcNow.AddDays(-100), Event = campaignEvent });
            Context.Tasks.Add(new AllReadyTask { Id = 2, Name = "Open Task", EndDateTime = DateTime.UtcNow.AddDays(100), Event = campaignEvent });

            Context.SaveChanges();
        }
    }
}
