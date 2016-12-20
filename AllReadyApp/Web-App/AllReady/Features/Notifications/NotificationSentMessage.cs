using AllReady.Models;
using MediatR;
using System;

namespace AllReady.Features.Notifications
{
    /// <summary>
    /// A message containing the properties for a notification which has been sent by the system
    /// </summary>
    public class NotificationSentMessage : IAsyncNotification
    {
        public NotificationMessageType MessageType { get; private set; }
        public NotificationMethod Method { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public bool ResponseRequired { get; private set; }
        public DateTime SentDateTime { get; private set; }
        public Guid? RequestId { get; private set; }
        public string UserId { get; private set; }

        private NotificationSentMessage()
        {
            SentDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new <see cref="NotificationSentMessage"/> for an SMS Message
        /// </summary>
        public static NotificationSentMessage SmsMessage(NotificationMessageType messageType, string phoneNumber, Guid? requestId = null, string userId = null, bool responseRequired = false)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            return new NotificationSentMessage
            {
                Method = NotificationMethod.Sms,
                MessageType = messageType,
                ResponseRequired = responseRequired,
                Phone = phoneNumber,
                UserId = userId,
                RequestId = requestId
            };
        }

        /// <summary>
        /// Creates a new <see cref="NotificationSentMessage"/> for an Email Message
        /// </summary>
        public static NotificationSentMessage EmailMessage(NotificationMessageType messageType, Guid? RequestId = null, string userId = null, bool responseRequired = false, string email = null)
        {
            return new NotificationSentMessage
            {
                Method = NotificationMethod.Email,
                MessageType = messageType,
                ResponseRequired = responseRequired,
                Email = email,
                UserId = userId,
                RequestId = RequestId
            };
        } 
    }
}
