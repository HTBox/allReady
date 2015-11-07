using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using MediatR;
using Microsoft.Framework.Configuration;
using SendGrid;
using Twilio;

namespace AllReady.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IConfiguration _config;
        private readonly IMediator _bus;

        public AuthMessageSender(IConfiguration config, IMediator bus)
        {
            _config = config;
            _bus = bus;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    EmailMessage = message,
                    EmailRecipients = new List<string> {email},
                    Subject = subject
                }
            };

            _bus.Send(command);
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {


            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    SmsMessage = message,
                    EmailRecipients = new List<string> { number },
                }
            };

            _bus.Send(command);

            return Task.FromResult(0);
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