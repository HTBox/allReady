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
            var taskSignup = new VolunteerTaskSignup { VolunteerTask = new VolunteerTask { Id = 1, Name = "TaskName" } };
            var sut = new TaskSignupViewModel(taskSignup);
            Assert.Equal(sut.TaskId, taskSignup.VolunteerTask.Id);
            Assert.Equal(sut.TaskName, taskSignup.VolunteerTask.Name);
        }

        [Fact]
        public void SetUserIdAndUserName_WhenTaskSignupsUserIsNotNull()
        {
            var taskSignup = new VolunteerTaskSignup { User = new ApplicationUser { Id = "1", UserName = "userName"} };
            var sut = new TaskSignupViewModel(taskSignup);
            Assert.Equal(sut.UserId, taskSignup.User.Id);
            Assert.Equal(sut.UserName, taskSignup.User.UserName);
        }
    }
}