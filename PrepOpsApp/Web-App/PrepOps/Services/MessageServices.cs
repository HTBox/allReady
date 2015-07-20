using Microsoft.Framework.Configuration;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Twilio;

namespace PrepOps.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IConfiguration _config;
        public AuthMessageSender(IConfiguration config)
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var myMessage = new SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new System.Net.Mail.MailAddress("Joe@contoso.com", "Joe S.");
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            var credentials = new NetworkCredential(
                _config["Authentication:SendGrid:UserName"],
                _config["Authentication:SendGrid:Password"]);
            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);
            // Send the email.
            if (transportWeb != null)
            {
                return transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            var twilio = new TwilioRestClient(
                _config["Authentication:Twilio:Sid"],
                _config["Authentication:Twilio:Token"]);
            var result = twilio.SendMessage(_config["Authentication:Twilio:PhoneNo"],
                number, message);
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
