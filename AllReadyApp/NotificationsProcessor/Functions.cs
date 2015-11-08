using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AllReady.Models.Notifications;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SendGrid;
using Twilio;

namespace NotificationsProcessor
{
    public class Functions
    {
        public static void ProcessSmsQueueMessage([QueueTrigger("sms-pending-deliveries")] string message, TextWriter log)
        {
            var queuedSmsMessage = JsonConvert.DeserializeObject<QueuedSmsMessage>(message);

            var accountSid = Environment.GetEnvironmentVariable("Authentication:Twilio:Sid");
            var authToken = Environment.GetEnvironmentVariable("Authentication:Twilio:Token");
            var from = Environment.GetEnvironmentVariable("Authentication:Twilio:PhoneNo");

            var to = queuedSmsMessage.Recipient;
            var text = queuedSmsMessage.Message;

            var twilio = new TwilioRestClient(accountSid, authToken);

            var smsMessage = twilio.SendMessage(from, to, text, "");
            if (smsMessage.RestException?.Message?.Length > 0)
            {
                log.WriteLine($"Error sending message: {smsMessage.RestException.Message}");
            }

        }

        public static void ProcessEmailQueueMessage([QueueTrigger("email-pending-deliveries")] string message, TextWriter log)
        {
            var emailMessage = JsonConvert.DeserializeObject<QueuedEmailMessage>(message);

            // Create the email object first, then add the properties.
            var from = Environment.GetEnvironmentVariable("Authentication:SendGrid:FromEmail");
            var myMessage = new SendGridMessage();
            myMessage.AddTo(emailMessage.Recipient);
            myMessage.From = new MailAddress(from, "AllReady");
            myMessage.Subject = emailMessage.Subject;
            myMessage.Text = emailMessage.Message;

            // Create credentials, specifying your user name and password.
            var username = Environment.GetEnvironmentVariable("Authentication:SendGrid:UserName");
            var password = Environment.GetEnvironmentVariable("Authentication:SendGrid:Password");
            var credentials = new NetworkCredential(username, password);

            // Create an Web transport for sending email, using credentials...
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(myMessage).Wait();

            log.WriteLine(message);
        }
    }
}
