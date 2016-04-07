using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Manage
{
    public class SendNewEmailAddressConfirmationEmailHandlerTests
    {
        [Fact]
        public async Task SendNewEmailAddressApprovalEamilHandlerInvokesSendEmailAsyncWithTheCorrectParameters()
        {
            var message = new SendNewEmailAddressConfirmationEmail { Email = "email", CallbackUrl = "CallbackUrl" };
            var emailMessage = $"Please confirm your new email address for your allReady account by clicking this link: <a href=\"{message.CallbackUrl}\">link</a>. Note that once confirmed your original email address will cease to be valid as your username.";

            var emailSender = new Mock<IEmailSender>();
            var sut = new SendNewEmailAddressConfirmationEmailHandler(emailSender.Object);
            await sut.Handle(message);

            emailSender.Verify(x => x.SendEmailAsync(message.Email, "Confirm your allReady account", emailMessage));
        }
    }
}
