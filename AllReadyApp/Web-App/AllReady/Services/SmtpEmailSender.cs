using System.Net.Mail;
using System.Threading.Tasks;
using AllReady.Models.Notifications;
using Newtonsoft.Json;

namespace AllReady.Services
{
    //this class is intended to be used by developers who want to run a local smtp server in order to work with notifications or confirmation email and sms messages
    //for smoke testing the system. To use this class, download a local smtp server (smtp4dev is an example of one of many of these: https://smtp4dev.codeplex.com/)
    //and change the container to use SmtpEmailSender instead of FakeQueueWriterService for IQueueStorageService in Startup.cs at this line:
    //services.AddTransient<IQueueStorageService, FakeQueueWriterService>();
    //just remember to change back to FakeQueueWriterService service before committing ;)
    public class SmtpEmailSender : IQueueStorageService
    {
        public Task SendMessageAsync(string queueName, string message)
        {
            MailMessage mailMessage;
            if (queueName == QueueStorageService.Queues.SmsQueue)
            {
                //turn sms message into email message
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