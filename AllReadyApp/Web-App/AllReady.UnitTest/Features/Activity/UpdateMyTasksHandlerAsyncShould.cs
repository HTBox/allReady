using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Event;
using AllReady.Models;
using AllReady.ViewModels;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class UpdateMyTasksHandlerAsyncShould
    {
        [Fact]
        public async Task InvokeGetUserWithTheCorrectUserId()
        {
            var message = new UpdateMyTasksCommandAsync { UserId = "1", TaskSignups = new List<TaskSignupViewModel>() };
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new UpdateMyTasksHandlerAsync(dataAccess.Object);
            await sut.Handle(message);

            dataAccess.Verify(x => x.GetUser(message.UserId), Times.Once);
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncForEachTaskSignupViewModelOnCommand()
        {
            var taskSignupViewModels = new List<TaskSignupViewModel> { new TaskSignupViewModel(), new TaskSignupViewModel() };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(new ApplicationUser());

            var sut = new UpdateMyTasksHandlerAsync(dataAccess.Object) { DateTimeUtcNow = () => DateTime.UtcNow };
            await sut.Handle(new UpdateMyTasksCommandAsync { TaskSignups = taskSignupViewModels });

            dataAccess.Verify(x => x.UpdateTaskSignupAsync(It.IsAny<TaskSignup>()), Times.Exactly(taskSignupViewModels.Count));
        }

        [Fact]
        public async Task InvokeUpdateTaskSignupAsyncWithTheCorrectParametersForEachTaskSignupViewModelOnCommand()
        {
            var user = new ApplicationUser();
            var dateTimeUtcNow = DateTime.UtcNow;
            var taskSignupViewModels = new List<TaskSignupViewModel>
            {
                new TaskSignupViewModel { Id = 1, StatusDescription = "statusDescription1", Status = "Status1", TaskId = 1, }
            };

            var message = new UpdateMyTasksCommandAsync { TaskSignups = taskSignupViewModels };

            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetUser(It.IsAny<string>())).Returns(user);

            var sut = new UpdateMyTasksHandlerAsync(dataAccess.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
            await sut.Handle(message);

            dataAccess.Verify(x => x.UpdateTaskSignupAsync(It.Is<TaskSignup>(y =>
                y.Id == taskSignupViewModels[0].Id &&
                y.StatusDateTimeUtc == dateTimeUtcNow &&
                y.StatusDescription == taskSignupViewModels[0].StatusDescription &&
                y.Status == taskSignupViewModels[0].Status &&
                y.Task.Id == taskSignupViewModels[0].TaskId &&
                y.User == user)));
        }
    }
}