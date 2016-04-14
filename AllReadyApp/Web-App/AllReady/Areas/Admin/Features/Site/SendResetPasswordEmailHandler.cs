using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class SendResetPasswordEmailHandler : AsyncRequestHandler<SendResetPasswordEmail>
    {
        private readonly IEmailSender emailSender;

        public SendResetPasswordEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendResetPasswordEmail message)
        {
            await emailSender.SendEmailAsync(message.Email, "Reset Password", $"Please reset your password by clicking here: <a href=\"{message.CallbackUrl}\">link</a>");
        }
    }
}
