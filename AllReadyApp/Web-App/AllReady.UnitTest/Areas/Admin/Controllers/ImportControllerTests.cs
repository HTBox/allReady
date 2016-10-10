using System;
using AllReady.Areas.Admin.Controllers;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ImportControllerTests
    {
        [Fact]
        public void IndexGetReturnsCorrectView()
        {
            var sut = new ImportController(null, null);
            var result = sut.Index() as ViewResult;

            Assert.Null(result.ViewName);
        }

        [Fact]
        public void IndexPostInvokesLogInformation_WhenFileIsNull()
        {
            const string userName = "UserName";

            var logger = new Mock<ILogger<ImportController>>();

            var sut = new ImportController(null, logger.Object);
            sut.SetFakeUserName(userName);
            sut.Index(null);

            var message = $"User {userName} attempted a file upload without specifying a file.";

            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(y => (string) y[0].Value == message), 
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void IndexPostRedirectsToCorrectAction_WhenFileIsNull()
        {
            var sut = new ImportController(null, Mock.Of<ILogger<ImportController>>());
            sut.SetFakeUser("1");
            var result = sut.Index(null) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(ImportController.Index));
        }
    }
}