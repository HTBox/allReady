using System.Threading.Tasks;
using AllReady.Features.Manage;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Manage
{
    public class SendAccountSecurityCodeSmsHandlerTests
    {
        [Fact]
        public async Task SendAccountSecurityCodeSmsHandlerInvokesSendSmsAsyncWithCorrectPhoneNumberAndMessage()
        {
            var message = new SendAccountSecurityCodeSms { PhoneNumber = "phoneNumber", Code = "code" };
            var smsMessage = $"Your allReady account security code is: {message.Code}";

            var smsSender = new Mock<ISmsSender>();
            var sut = new SendAccountSecurityCodeSmsHandler(smsSender.Object);
            await sut.Handle(message);

            smsSender.Verify(x => x.SendSmsAsync(message.PhoneNumber, smsMessage));
        }
    }
}
