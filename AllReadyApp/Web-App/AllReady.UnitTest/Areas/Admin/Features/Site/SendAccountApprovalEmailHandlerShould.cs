using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Site
{
    public class SendAccountApprovalEmailHandlerShould
    {
        [Fact]
        public async Task InvokeSendEmailAsyncWithTheCorrectParameters()
        {
            var message = new SendAccountApprovalEmail { Email = "email", CallbackUrl = "callBackUrl" };
            var emailMessage = $"Your account has been approved by an administrator. Please <a href=\"{message.CallbackUrl}\">Click here to Log in</a>";

            var emailSender = new Mock<IEmailSender>();
            var sut = new SendAccountApprovalEmailHandler(emailSender.Object);
            await sut.Handle(message);

            emailSender.Verify(x => x.SendEmailAsync(message.Email, "Account Approval", emailMessage), Times.Once);
        }
    }
}
