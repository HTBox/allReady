using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Manage
{
    public class SendConfirmAccountEmailHandler : AsyncRequestHandler<SendConfirmAccountEmail>
    {
        private readonly IEmailSender emailSender;
       
        public SendConfirmAccountEmailHandler(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        protected override async Task HandleCore(SendConfirmAccountEmail message)
        {
            await emailSender.SendEmailAsync(message.Email, "Confirm your allReady account",
                $"Please confirm your allReady account by clicking this link: <a href=\"{message.CallbackUrl}\">{message.CallbackUrl}</a>");
        }
    }
}
