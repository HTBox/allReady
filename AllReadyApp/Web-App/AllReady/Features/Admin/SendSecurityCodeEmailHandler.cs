using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Admin
{
    public class SendSecurityCodeEmailHandler : AsyncRequestHandler<SendSecurityCodeEmail>
    {
        private readonly IEmailSender emailSender;

        public SendSecurityCodeEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendSecurityCodeEmail message)
        {
            await emailSender.SendEmailAsync(message.Email, "Security Code", $"Your security code is: {message.Token}")
                .ConfigureAwait(false);
        }
    }
}
