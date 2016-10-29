using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Manage
{
    public class SendNewEmailAddressConfirmationEmailHandler : AsyncRequestHandler<SendNewEmailAddressConfirmationEmail>
    {
        private readonly IEmailSender emailSender;

        public SendNewEmailAddressConfirmationEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendNewEmailAddressConfirmationEmail message)
        {
            await emailSender.SendEmailAsync(message.Email, "Confirm your allReady account",
                $"Please confirm your new email address for your allReady account by clicking this link: <a href=\"{message.CallbackUrl}\">link</a>. Note that once confirmed your original email address will cease to be valid as your username.");
        }
    }
}
