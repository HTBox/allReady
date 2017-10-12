using AllReady.Core.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Twilio;

namespace NotificationProcessing
{
    public static class ProcessSmsQueueMessage
    {
        [FunctionName("ProcessSmsQueueMessage")]
        [StorageAccount("AzureWebJobsStorage")]
        public static void Run([QueueTrigger("sms-pending-deliveries")] string queueItem,
            [TwilioSms(AccountSidSetting = "Authentication:Twilio:Sid",
                AuthTokenSetting = "Authentication:Twilio:Token",
                From = "%Authentication:Twilio:PhoneNo%")] out SMSMessage message, TraceWriter log)
        {
            var queuedSmsMessage = JsonConvert.DeserializeObject<QueuedSmsMessage>(queueItem);

            log.Info($"Sending SMS message `{queuedSmsMessage.Message}` to `{queuedSmsMessage.Recipient}`");

            message = new SMSMessage()
            {
                Body = queuedSmsMessage.Message,
                To = queuedSmsMessage.Recipient
            };
        }
    }
}
