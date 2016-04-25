using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Admin
{
    public class SendAccountConfirmationEmailHandler : AsyncRequestHandler<SendAccountConfirmationEmail>
    {
        private readonly IEmailSender emailSender;

        public SendAccountConfirmationEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendAccountConfirmationEmail message)
        {
            await emailSender.SendEmailAsync(message.Email, "Confirm your account", $"Please confirm your account by clicking this <a href=\"{message.CallbackUrl}\">link</a>")
                .ConfigureAwait(false);
        }
    }
}
