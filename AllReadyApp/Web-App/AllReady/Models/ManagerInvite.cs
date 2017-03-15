using System;

namespace AllReady.Models
{
    /// <summary>
    /// Represents an invite to a management role within allReady
    /// </summary>
    public abstract class ManagerInvite
    {
        public int Id { get; set; }
        public string InviteeEmailAddress { get; set; }

        public DateTime SentDateTimeUtc { get; set; }
        public DateTime? AcceptedDateTimeUtc { get; set; }
        public DateTime? RejectedDateTimeUtc { get; set; }
        public DateTime? RevokedDateTimeUtc { get; set; }

        public string CustomMessage { get; set; }
        
        public string SenderUserId { get; set; }
        public ApplicationUser SenderUser { get; set; }
    }

    /// <summary>
    /// Represents an invite to an event management role within allReady
    /// </summary>
    public class EventManagerInvite : ManagerInvite
    {
        public int EventId { get; set; }
        public Event Event { get; set; }
    }

    /// <summary>
    /// Represents an invite to a campaign management role within allReady
    /// </summary>
    public class CampaignManagerInvite : ManagerInvite
    {
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}
