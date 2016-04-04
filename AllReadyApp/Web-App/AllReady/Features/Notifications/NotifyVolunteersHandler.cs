﻿using System.Threading.Tasks;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Newtonsoft.Json;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersHandler : AsyncRequestHandler<NotifyVolunteersCommand>
    {
        private readonly IQueueStorageService _storageService;
        public NotifyVolunteersHandler(IQueueStorageService storageService)
        {
            _storageService = storageService;
        }

        protected override async Task HandleCore(NotifyVolunteersCommand message)
        {
            // TODO: both SMS and email sent to the same email service?

            // push messages to azure
            foreach (var recipient in message.ViewModel.SmsRecipients)
            {
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = recipient,
                    Message = message.ViewModel.SmsMessage
                };
                var sms = JsonConvert.SerializeObject(queuedSms);
                await _storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
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
                await _storageService.SendMessageAsync(QueueStorageService.Queues.EmailQueue, email);
            }
        }
    }
}