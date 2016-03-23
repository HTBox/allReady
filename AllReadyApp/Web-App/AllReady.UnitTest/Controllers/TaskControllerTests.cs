using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class TaskControllerTests
    {
        [Fact]
        public void CreateInvokesGetActivityWithTheCorrectActivityId()
        {
            const int activityId = 1;
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new TaskController(dataAccess.Object, null);
            sut.Create(activityId);

            dataAccess.Verify(x => x.GetActivity(activityId), Times.Once);
        }

        [Fact]
        public void CreateReturnsHttpUnauthorizedResultWhenActivityIsNull()
        {
            var sut = new TaskController(Mock.Of<IAllReadyDataAccess>(), null);
            var result = sut.Create(It.IsAny<int>());

            Assert.IsType<HttpUnauthorizedResult>(result);
        }

        [Fact]
        public void CreateReturnsHttpUnauthorizedResultWhen()
        {
        }
    }
}
