using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;

namespace AllReady.Services
{
    public class FakeQueueWriterService : IQueueStorageService
    {
        private readonly ILogger _logger;

        public FakeQueueWriterService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(FakeQueueWriterService).FullName);
        }

        public void SendMessage(string queueName, string message)
        {
            _logger.LogInformation($"Message to {queueName} queue: {message}");
        }
    }
}
