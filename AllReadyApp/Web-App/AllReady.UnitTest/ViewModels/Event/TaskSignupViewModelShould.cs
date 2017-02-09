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
            var volunteerTaskSignup = new VolunteerTaskSignup { VolunteerTask = new VolunteerTask { Id = 1, Name = "TaskName" } };
            var sut = new VolunteerTaskSignupViewModel(volunteerTaskSignup);
            Assert.Equal(sut.VolunteerTaskId, volunteerTaskSignup.VolunteerTask.Id);
            Assert.Equal(sut.VolunteerTaskName, volunteerTaskSignup.VolunteerTask.Name);
        }

        [Fact]
        public void SetUserIdAndUserName_WhenTaskSignupsUserIsNotNull()
        {
            var volunteerTaskSignup = new VolunteerTaskSignup { User = new ApplicationUser { Id = "1", UserName = "userName"} };
            var sut = new VolunteerTaskSignupViewModel(volunteerTaskSignup);
            Assert.Equal(sut.UserId, volunteerTaskSignup.User.Id);
            Assert.Equal(sut.UserName, volunteerTaskSignup.User.UserName);
        }
    }
}