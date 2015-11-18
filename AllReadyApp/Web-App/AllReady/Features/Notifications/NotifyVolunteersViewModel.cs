using System.Collections.Generic;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersViewModel
    {
        public NotifyVolunteersViewModel()
        {
            SmsRecipients = new List<string>();
            EmailRecipients = new List<string>();
        }

        public string SmsMessage { get; set; }
        public List<string> SmsRecipients { get; set; }
        public string EmailMessage { get; set; }
        public List<string> EmailRecipients { get; set; }
        public string Subject { get; set; }
        public string HtmlMessage { get; set; }
    }
}