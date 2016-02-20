using System.Threading.Tasks;

namespace AllReady.Services
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string queueName, string message);
    }
}