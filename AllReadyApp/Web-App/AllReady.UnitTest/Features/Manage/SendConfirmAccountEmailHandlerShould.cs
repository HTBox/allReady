﻿using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Manage
{
    public class SendConfirmAccountEmailHandlerShould
    {
        [Fact]
        public async Task SendNewEmailAddressApprovalEamilHandlerInvokesSendEmailAsyncWithTheCorrectParameters()
        {
            var message = new SendConfirmAccountEmail { Email = "email", CallbackUrl = "CallbackUrl" };
            var emailMessage = $"Please confirm your allReady account by clicking this link: <a href=\"{message.CallbackUrl}\">{message.CallbackUrl}</a>";

            var emailSender = new Mock<IEmailSender>();
            var sut = new SendConfirmAccountEmailHandler(emailSender.Object);
            await sut.Handle(message);

            emailSender.Verify(x => x.SendEmailAsync(message.Email, "Confirm your allReady account", emailMessage));
        }
    }
}
