using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Manage
{
    public class SendAccountSecurityCodeSmsHandler : AsyncRequestHandler<SendAccountSecurityCodeSms>
    {
        private readonly ISmsSender smsSender;

        public SendAccountSecurityCodeSmsHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        protected override async Task HandleCore(SendAccountSecurityCodeSms message)
        {
            await smsSender.SendSmsAsync(message.PhoneNumber, $"Your allReady account security code is: {message.Code}");
        }
    }
}
