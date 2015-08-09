using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Framework.Configuration;
using SendGrid;
using System.Net;

namespace AllReady.Services
{
    public class SendGridDevelopmentWeb : ITransport
    {
        private IConfiguration _config;
        public SendGridDevelopmentWeb(IConfiguration config)
        {
            _config = config;
        }

        public Task DeliverAsync(ISendGrid message)
        {
            var client = new SmtpClient();
            var resultObject = new object();
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = _config.Get("DevelopmentEmailFolder");
            client.SendAsync(new MailMessage(message.From.Address, message.To.FirstOrDefault().Address, message.Subject, message.Html), resultObject);
            return Task.FromResult(resultObject);
        }
    }

    public static class ITransportAdapter
    {
        public static ITransport SendGridTransport { private get; set; }
        public static IConfiguration Config { private get; set; }
        public static NetworkCredential Credentials { private get; set; }

        private static bool isDebugMode = false;

        static ITransportAdapter()
        {
            #if DEBUG
                isDebugMode = true;
            #endif
        }

        public static ITransport Create()
        {
            if(SendGridTransport != null)
            {
               return SendGridTransport;
            }

            if (isDebugMode && Config != null)
            {
                return new SendGridDevelopmentWeb(Config);
            }
            else if (Credentials != null)
            {
                return new Web(Credentials);
            }

            throw new Exception("SendGridWebAdapter requires an instance of ITransport, IConfiguration (and must be in debug mode), or Network Credentials");
        }
    }
}