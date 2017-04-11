using AllReady.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ErrorControllerTests
    {
        [Fact]
        public void ErrorReturnsTheCorrectView_WhenStatusCodeIsNull()
        {
            var controller = new ErrorController();
            var result = (ViewResult) controller.Error(null);
            Assert.Equal("~/Views/Shared/Error.cshtml", result.ViewName);
        }

        [Fact]
        public void ErrorReturnsTheCorrectView_WhenStatusCodeIs404()
        {
            var controller = new ErrorController();
            var result = (ViewResult) controller.Error(404);
            Assert.Equal("404", result.ViewName);
        }
    }
}
