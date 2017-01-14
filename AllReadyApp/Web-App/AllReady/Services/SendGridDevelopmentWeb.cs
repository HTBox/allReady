using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;

namespace AllReady.Services
{
    //public class SendGridDevelopmentWeb : ITransport
    //{
    //    private readonly EmailSettings _settings;

    //    public SendGridDevelopmentWeb(IOptions<EmailSettings> options)
    //    {
    //        _settings = options.Value;
    //    }

    //    public Task DeliverAsync(ISendGrid message)
    //    {
    //        var client = new SmtpClient
    //        {
    //            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
    //            PickupDirectoryLocation = _settings.EmailFolder
    //        };
    //        var toAddress = message.To.FirstOrDefault();
    //        if (toAddress == null)
    //        {
    //            throw new InvalidOperationException("Can't send email without addressee.");
    //        }
    //        return client.SendMailAsync(new MailMessage(message.From.Address, toAddress.Address, message.Subject, message.Html));
    //    }
    //}
}