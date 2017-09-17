using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AllReady.Services
{
    public class FakeQueueWriterService : IQueueStorageService
    {
        private readonly ILogger _logger;

        public FakeQueueWriterService(ILogger<FakeQueueWriterService> logger)
        {
            _logger = logger;
        }

        public Task SendMessageAsync(string queueName, string message)
        {
            _logger.LogInformation("Message to {queueName} queue: {message}", queueName, message);
            return Task.CompletedTask;
        }
    }
}
