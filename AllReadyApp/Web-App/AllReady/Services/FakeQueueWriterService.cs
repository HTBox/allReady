using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace AllReady.Services
{
    public class FakeQueueWriterService : IQueueStorageService
    {
        private readonly ILogger _logger;
        private readonly IOptions<AzureStorageSettings> _options;

        public FakeQueueWriterService(ILogger<FakeQueueWriterService> logger, IOptions<AzureStorageSettings> options)
        {
            _logger = logger;
            _options = options;
        }

        public void SendMessage(string queueName, string message)
        {
            _logger.LogInformation("Message to {queueName} queue: {message}", queueName, message);
        }
    }
}
