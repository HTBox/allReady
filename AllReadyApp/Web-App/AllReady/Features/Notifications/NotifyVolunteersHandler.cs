using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Newtonsoft.Json;
using RestSharp;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersHandler : RequestHandler<NotifyVolunteersCommand>
    {
        private readonly IQueueStorageService _storageService;
        public NotifyVolunteersHandler(IQueueStorageService storageService)
        {
            _storageService = storageService;
        }

        protected override void HandleCore(NotifyVolunteersCommand message)
        {
            // push messages to azure
            foreach (var recipient in message.ViewModel.SmsRecipients)
            {
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = recipient,
                    Message = message.ViewModel.SmsMessage
                };
                var sms = JsonConvert.SerializeObject(queuedSms);
                _storageService.SendMessage(QueueStorageService.Queues.SmsQueue, sms);
            }

            foreach (var recipient in message.ViewModel.EmailRecipients)
            {
                var queuedEmail = new QueuedEmailMessage
                {
                    Recipient = recipient,
                    Message = message.ViewModel.EmailMessage,
                    HtmlMessage = message.ViewModel.HtmlMessage,
                    Subject = message.ViewModel.Subject
                };
                var email = JsonConvert.SerializeObject(queuedEmail);
                _storageService.SendMessage(QueueStorageService.Queues.EmailQueue, email);
            }

        }
    }
}
