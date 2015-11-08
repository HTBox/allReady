using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
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

        public QueueStorageService(IConfiguration config)
        {
            var storageAccount = CloudStorageAccount.Parse(config["Data:Storage:AzureStorage"]);
            _client = storageAccount.CreateCloudQueueClient();
        }

        public void SendMessage(string queueName, string message)
        {
            var queue = _client.GetQueueReference(queueName);
            queue.CreateIfNotExistsAsync().Wait();
            queue.AddMessageAsync(new CloudQueueMessage(message)).Wait();
        }
    }
}
