using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    /// <summary>
    /// Represents an invite to a management role within allReady
    /// </summary>
    public abstract class ManagerInvite
    {
        /// <summary>
        /// The ID of the invite
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The email address of the invitee
        /// </summary>
        public string InviteeEmailAddress { get; set; }

        /// <summary>
        /// The date and time when the invite was sent
        /// </summary>
        public DateTime SentDateTimeUtc { get; set; }

        /// <summary>
        /// The date and time (UTC) when the invite was accepted (if accepted)
        /// </summary>
        public DateTime? AcceptedDateTimeUtc { get; set; }

        /// <summary>
        /// The date and time (UTC) when the invite was rejected (if rejected)
        /// </summary>
        public DateTime? RejectedDateTimeUtc { get; set; }

        /// <summary>
        /// The date and time (UTC) when the invite was revoked (if revoked)
        /// </summary>
        public DateTime? RevokedDateTimeUtc { get; set; }

        /// <summary>
        /// A custom message included in the email sent to the invitee
        /// </summary>
        public string CustomMessage { get; set; }
        
        /// <summary>
        /// The ID of the sender
        /// </summary>
        public string SenderUserId { get; set; }

        /// <summary>
        /// A navigation property to the sending <see cref="ApplicationUser"/>
        /// </summary>
        public ApplicationUser SenderUser { get; set; }

        /// <summary>
        /// Indicates of the invite is accepted
        /// </summary>
        public bool IsAccepted => AcceptedDateTimeUtc.HasValue;
        
        /// <summary>
        /// Indicates of the invite is rejected
        /// </summary>
        public bool IsRejected => RejectedDateTimeUtc.HasValue;

        /// <summary>
        /// Indicates of the invite is revoked
        /// </summary>
        public bool IsRevoked => RevokedDateTimeUtc.HasValue;

        /// <summary>
        /// Indicates of the invite is pending
        /// </summary>
        public bool IsPending => !IsAccepted && !IsRejected && !IsRevoked;
    }

    /// <summary>
    /// Represents an invite to an event management role within allReady
    /// </summary>
    public class EventManagerInvite : ManagerInvite
    {
        /// <summary>
        /// The ID of the event the invitee is invited to manage
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// A navigation property to the event the invitee is invited to manage
        /// </summary>
        public Event Event { get; set; }
    }

    /// <summary>
    /// Represents an invite to a campaign management role within allReady
    /// </summary>
    public class CampaignManagerInvite : ManagerInvite
    {
        /// <summary>
        /// The ID of the campaign the invitee is invited to manage
        /// </summary>
        public int CampaignId { get; set; }

        /// <summary>
        /// A navigation property to the campaign the invitee is invited to manage
        /// </summary>
        public Campaign Campaign { get; set; }
    }
}
