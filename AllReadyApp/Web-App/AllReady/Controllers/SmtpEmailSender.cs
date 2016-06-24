using System.Net.Mail;
using System.Threading.Tasks;
using AllReady.Models.Notifications;
using AllReady.Services;
using Newtonsoft.Json;

namespace AllReady.Controllers
{
    public class SmtpEmailSender : IQueueStorageService
    {
        public Task SendMessageAsync(string queueName, string message)
        {
            MailMessage mailMessage;
            if (queueName == QueueStorageService.Queues.SmsQueue)
            {
                var sms = JsonConvert.DeserializeObject<QueuedSmsMessage>(message);
                mailMessage = new MailMessage("fakesender@fakesender.com", "fakereceiver0@fakereceiver.com")
                {
                    Subject = "SmsMessagePretendingToBeAnEmailMessage",
                    Body = sms.Message,
                };
                SendMessage(mailMessage);
            }
            else
            {
                var email = JsonConvert.DeserializeObject<QueuedEmailMessage>(message);
                mailMessage = new MailMessage("fakeemailaddress@fakeemailaddress.com", email.Recipient)
                {
                    Subject = email.Subject,
                    Body = email.Message,
                    IsBodyHtml = true
                };
                SendMessage(mailMessage);
            }

            return Task.FromResult(0);
        }

        protected virtual void SendMessage(MailMessage message)
        {
            new SmtpClient { Host = "localhost" }.Send(message);
        }
    }
}
