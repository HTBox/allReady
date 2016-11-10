using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Manage
{
    public class SendAccountSecurityTokenSmsHandlerShould
    {
        [Fact]
        public async Task SendAccountSecurityTokenSmsHandlerInvokesSendSmsAsyncWithCorrectPhoneNumberAndMessage()
        {
            var message = new SendAccountSecurityTokenSms { PhoneNumber = "phoneNumber", Token = "code" };
            var smsMessage = $"Your allReady account security code is: {message.Token}";

            var smsSender = new Mock<ISmsSender>();
            var sut = new SendAccountSecurityTokenSmsHandler(smsSender.Object);
            await sut.Handle(message);

            smsSender.Verify(x => x.SendSmsAsync(message.PhoneNumber, smsMessage));
        }
    }
}
