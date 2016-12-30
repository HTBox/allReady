using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class SendAccountApprovalEmailCommandHandler : AsyncRequestHandler<SendAccountApprovalEmailCommand>
    {
        private readonly IEmailSender emailSender;

        public SendAccountApprovalEmailCommandHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendAccountApprovalEmailCommand message)
        {
            await emailSender.SendEmailAsync(message.Email, "Account Approval",
                $"Your account has been approved by an administrator. Please <a href=\"{message.CallbackUrl}\">Click here to Log in</a>");
        }
    }
}
