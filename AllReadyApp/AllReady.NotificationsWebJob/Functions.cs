using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AllReady.Core.Notifications;
using Microsoft.Azure.WebJobs;
using SendGrid;
using Twilio;
using Newtonsoft.Json;

namespace AllReady.NotificationsWebJob
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

        public static async Task ProcessEmailQueueMessage([QueueTrigger("email-pending-deliveries")] string message, TextWriter log)
        {
            var emailMessage = JsonConvert.DeserializeObject<QueuedEmailMessage>(message);

            // Create the email object first, then add the properties.
            var from = GuardAgainstInvalidEmailAddress(EnvironmentHelper.TryGetEnvironmentVariable("Authentication:SendGrid:FromEmail"));
            var email = new SendGridMessage();
            email.AddTo(emailMessage.Recipient);
            email.From = new MailAddress(from, "AllReady");
            email.Subject = emailMessage.Subject;
            email.Html = emailMessage.HtmlMessage;
            email.Text = emailMessage.Message;

            // Create credentials, specifying your user name and password.
            var username = EnvironmentHelper.TryGetEnvironmentVariable("Authentication:SendGrid:UserName");
            var password = EnvironmentHelper.TryGetEnvironmentVariable("Authentication:SendGrid:Password");
            var credentials = new NetworkCredential(username, password);

            // Create an Web transport for sending email, using credentials...
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(email);

            await log.WriteLineAsync($"Sent email with subject `{email.Subject}` to `{emailMessage.Recipient}`");
        }

        private static string GuardAgainstInvalidEmailAddress(string @from)
        {
            if (string.IsNullOrWhiteSpace(@from))
            {
                throw new InvalidOperationException(
                    "Environment variable `Authentication:SendGrid:FromEmail` is missing or contains invalid entry.");
            }

            return @from;
        }
    }
}