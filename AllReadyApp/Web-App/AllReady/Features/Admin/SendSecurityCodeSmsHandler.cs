using System.Threading.Tasks;
using AllReady.Services;
using MediatR;

namespace AllReady.Features.Admin
{
    public class SendSecurityCodeSmsHandler : AsyncRequestHandler<SendSecurityCodeSms>
    {
        private readonly ISmsSender smsSender;

        public SendSecurityCodeSmsHandler(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
        }

        protected override async Task HandleCore(SendSecurityCodeSms message)
        {
            await smsSender.SendSmsAsync(message.PhoneNumber, $"Your security code is: {message.Token}")
                .ConfigureAwait(false);
        }
    }
}
