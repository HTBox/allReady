using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Core.Notifications;
using AllReady.Features.Notifications;
using MediatR;
using Newtonsoft.Json;

namespace AllReady.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IMediator _mediator;
        private readonly IQueueStorageService _storageService;

        public AuthMessageSender(IMediator mediator, IQueueStorageService storageService)
        {
            _mediator = mediator;
            _storageService = storageService;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message,
                    EmailRecipients = new List<string> {email},
                    HtmlMessage = message,
                    Subject = subject
                }
            };

            return _mediator.SendAsync(command);
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            var queuedSms = new QueuedSmsMessage
            {
                Recipient = phoneNumber,
                Message = message
            };
            var sms = JsonConvert.SerializeObject(queuedSms);
            await _storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
        }

        public async Task SendSmsAsync(List<string> phoneNumbers, string message)
        {
            foreach (var phoneNumber in phoneNumbers)
            {
                await SendSmsAsync(phoneNumber, message);
            }
        }
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendSmsAsync(List<string> phoneNumbers, string message);
    }
}