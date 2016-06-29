using AllReady.Models;
using AllReady.ViewModels.Event;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Event
{
    public class TaskSignupViewModelShould
    {
        [Fact]
        public void SetTaskIdAndTaskName_WhenTaskSignupsTaskIsNotNull()
        {
            var taskSignup = new TaskSignup { Task = new AllReadyTask { Id = 1, Name = "TaskName" } };
            var sut = new TaskSignupViewModel(taskSignup);
            Assert.Equal(sut.TaskId, taskSignup.Task.Id);
            Assert.Equal(sut.TaskName, taskSignup.Task.Name);
        }

        [Fact]
        public void SetUserIdAndUserName_WhenTaskSignupsUserIsNotNull()
        {
            var taskSignup = new TaskSignup { User = new ApplicationUser { Id = "1", UserName = "userName"} };
            var sut = new TaskSignupViewModel(taskSignup);
            Assert.Equal(sut.UserId, taskSignup.User.Id);
            Assert.Equal(sut.UserName, taskSignup.User.UserName);
        }
    }
}
