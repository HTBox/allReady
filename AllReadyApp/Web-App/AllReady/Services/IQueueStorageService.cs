namespace AllReady.Services
{
    public interface IQueueStorageService
    {
        void SendMessage(string queueName, string message);
    }
}