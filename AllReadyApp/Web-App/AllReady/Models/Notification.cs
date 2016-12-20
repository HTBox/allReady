using AllReady.Features.Notifications;
using System;

namespace AllReady.Models
{
    /// <summary>
    /// Holds data about any notifications sent via allReady
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// A unique ID for the notification
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The method for the notification (i.e. email or SMS)
        /// </summary>
        /// <remarks>Storing as string (rather than enum) for log readability</remarks>
        public string Method { get; set; }

        /// <summary>
        /// The type of message being sent
        /// </summary>
        /// <remarks>Storing as string (rather than enum) for log readability</remarks>
        public string MessageType { get; set; }

        /// <summary>
        /// The date and time when the message was sent (UTC)
        /// </summary>
        public DateTime SentDateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// If the notification is via SMS we store the recipient phone number (required if SMS type)
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// If the notification is via email we store the recipient email address (required if Email type)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Identifies whether this notification expects the recipient to respond
        /// </summary>
        public bool ResponseRequired { get; set; }

        #region Navigational Properties

        // Navigational Properties used for EF mapping and relationship navigation

        /// <summary>
        /// An optional request id for the notification if the request can be associated back to an <see cref="Request"/> 
        /// </summary>
        public Guid? RequestId { get; set; }

        /// <summary>
        /// An optional <see cref="Request"/> that the notification is associated to
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// An optional user id for the notification if the request can be associated back to an <see cref="ApplicationUser"/> 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// An optional <see cref="ApplicationUser"/> that the notification is associated to
        /// </summary>
        public ApplicationUser User { get; set; }

        #endregion

        /// <summary>
        /// Creates a new <see cref="Notification"/> instance using the properties from an <see cref="NotificationSentMessage"/> 
        /// </summary>
        public static Notification FromAddNotificationLogMessage(NotificationSentMessage message)
        {
            return new Notification
            {
                Method = message.Method.ToString(),
                MessageType = message.MessageType.ToString(),
                SentDateTime = message.SentDateTime,
                PhoneNumber = message.Phone,
                Email = message.Email,
                ResponseRequired = message.ResponseRequired,
                RequestId = message.RequestId,
                UserId = message.UserId,
            };
        }
    }

    /// <summary>
    /// Identifies the possible notification methods that are in use within the application
    /// </summary>
    public enum NotificationMethod
    {
        /// <summary>
        /// Message is sent via email system
        /// </summary>
        Email,

        /// <summary>
        /// Message is sent via the mobile/cell phone network (SMS)
        /// </summary>
        Sms
    }

    /// <summary>
    /// Defines the various messages that can be sent via the application
    /// </summary>
    public enum NotificationMessageType
    {
        /// <summary>
        /// A request has been assigned to an itinerary to confirm availability
        /// </summary>
        InitialItineraryAssignment,

        /// <summary>
        /// A 7 day reminder before scheduled request date to re-confirm availability
        /// </summary>
        SevenDayReminder,

        /// <summary>
        /// A 1 day reminder before scheduled request date to re-confirm availability
        /// </summary>
        OneDayReminder,

        /// <summary>
        /// A notification sent on the day of request fullfillment when the prior request has been completed
        /// </summary>
        NextRequestForItinerary
    }
}
