using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class SendResetPasswordEmailCommandHandler : AsyncRequestHandler<SendResetPasswordEmailCommand>
    {
        private readonly IEmailSender emailSender;

        public SendResetPasswordEmailCommandHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendResetPasswordEmailCommand message)
        {
            await emailSender.SendEmailAsync(message.Email, "Reset Password", $"Please reset your password by clicking here: <a href=\"{message.CallbackUrl}\">link</a>");
        }
    }
}
