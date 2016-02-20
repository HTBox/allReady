using Microsoft.Extensions.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AllReady.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        public class Queues
        {
            public const string SmsQueue = "sms-pending-deliveries";
            public const string EmailQueue = "email-pending-deliveries";
        }

        private readonly CloudQueueClient _client;

        public QueueStorageService(IOptions<AzureStorageSettings> options)
        {
            var storageAccount = CloudStorageAccount.Parse(options.Value.AzureStorage);
            _client = storageAccount.CreateCloudQueueClient();
        }

        public void SendMessage(string queueName, string message)
        {
            var queue = _client.GetQueueReference(queueName);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            queue.AddMessageAsync(new CloudQueueMessage(message)).GetAwaiter().GetResult();
        }
    }
}
