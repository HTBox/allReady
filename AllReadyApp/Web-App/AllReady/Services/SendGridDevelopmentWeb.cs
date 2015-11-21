using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SendGrid;
using System.Net;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Services
{
    public class SendGridDevelopmentWeb : ITransport
    {
        private readonly EmailSettings _settings;

        public SendGridDevelopmentWeb(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public Task DeliverAsync(ISendGrid message)
        {
            var client = new SmtpClient();
            var resultObject = new object();
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = _settings.EmailFolder;
            client.SendAsync(new MailMessage(message.From.Address, message.To.FirstOrDefault().Address, message.Subject, message.Html), resultObject);
            return Task.FromResult(resultObject);
        }
    }
}