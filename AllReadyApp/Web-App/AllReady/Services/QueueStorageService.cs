using System.Threading.Tasks;
using AllReady.Configuration;
using Microsoft.Extensions.Options;
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

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queue = _client.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(new CloudQueueMessage(message));
        }
    }
}
