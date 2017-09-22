using System;
using AllReady.Core.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace NotificationProcessing
{
    public static class ProcessEmailQueueMessage
    {
        [FunctionName("ProcessEmailQueueMessage")]
        [StorageAccount("AzureWebJobsStorage")]
        public static void Run([QueueTrigger("email-pending-deliveries")] string queueItem,
            [SendGrid(ApiKey = "Authentication:SendGrid:ApiKey")] out Mail message, TraceWriter log)
        {
            var queuedEmailMessage = JsonConvert.DeserializeObject<QueuedEmailMessage>(queueItem);

            var from = GuardAgainstInvalidEmailAddress(
                Environment.GetEnvironmentVariable("Authentication:SendGrid:FromEmail"));

            log.Info($"Sending email with subject `{queuedEmailMessage.Subject}` to `{queuedEmailMessage.Recipient}`");

            message = new Mail
            {
                From = new Email(from, "AllReady"),
                Subject = queuedEmailMessage.Subject
            };

            if (queuedEmailMessage.Message != null)
            {
                message.AddContent(new Content
                {
                    Type = "text/plain",
                    Value = queuedEmailMessage.Message
                });
            }

            if (queuedEmailMessage.HtmlMessage != null)
            {
                message.AddContent(new Content
                {
                    Type = "text/html",
                    Value = queuedEmailMessage.HtmlMessage
                });
            }

            var personalization = new Personalization();
            personalization.AddTo(new Email(queuedEmailMessage.Recipient));
            message.AddPersonalization(personalization);
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
