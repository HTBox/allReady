using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (queueName == QueueStorageService.Queues.SmsQueue)
                return Task.FromResult(0);

            var email = JsonConvert.DeserializeObject<QueuedEmailMessage>(message);

            var mailMessage = new MailMessage("fakeemailaddress@fakeemailaddress.com", email.Recipient)
            {
                Body = email.Message,
                IsBodyHtml = true,
                Subject = email.Subject
            };
            this.SendMessage(mailMessage);

            return Task.FromResult(0);
        }

        protected virtual void SendMessage(MailMessage message)
        {
            new SmtpClient { Host = "localhost" }.Send(message);
        }
    }
}
