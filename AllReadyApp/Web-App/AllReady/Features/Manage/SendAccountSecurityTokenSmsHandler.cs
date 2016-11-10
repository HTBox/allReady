using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Manage
{
    public class SendAccountSecurityTokenSmsHandler : AsyncRequestHandler<SendAccountSecurityTokenSms>
    {
        private readonly ISmsSender smsSender;

        public SendAccountSecurityTokenSmsHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        protected override async Task HandleCore(SendAccountSecurityTokenSms message)
        {
            await smsSender.SendSmsAsync(message.PhoneNumber, $"Your allReady account security code is: {message.Token}");
        }
    }
}
