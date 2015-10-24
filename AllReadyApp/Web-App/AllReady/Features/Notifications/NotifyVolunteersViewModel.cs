using System.Collections.Generic;

namespace AllReady.Features.Notifications
{
    public class NotifyVolunteersViewModel
    {
        public string SmsMessage { get; set; }
        public List<string> SmsRecipients { get; set; }
        public string EmailMessage { get; set; }
        public List<string> EmailRecipients { get; set; }
    }
}