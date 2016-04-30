using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Admin
{
    public class SendApproveOrganizationUserAccountEmailHandler : AsyncRequestHandler<SendApproveOrganizationUserAccountEmail>
    {
        private readonly IEmailSender emailSender;

        public SendApproveOrganizationUserAccountEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendApproveOrganizationUserAccountEmail message)
        {
            await emailSender.SendEmailAsync(message.DefaultAdminUsername, "Approve organization user account", 
                $"Please approve this account by clicking this <a href=\"{message.CallbackUrl}\">link</a>")
                .ConfigureAwait(false);
        }
    }
}
