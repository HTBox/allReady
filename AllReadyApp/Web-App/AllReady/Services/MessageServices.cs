using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;

namespace AllReady.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IMediator _mediator;

        public AuthMessageSender(IMediator mediator)
        {
            _mediator = mediator;
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

        public Task SendSmsAsync(string number, string message)
        {
            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = message,
                    SmsRecipients = new List<string> { number },
                }
            };

            return _mediator.SendAsync(command);
        }
    }
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}