namespace AllReady.Models.Notifications
{
    public class QueuedEmailMessage
    {
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }

        // todo: augment this class to support mail service templates and/or html messages
    }
}
