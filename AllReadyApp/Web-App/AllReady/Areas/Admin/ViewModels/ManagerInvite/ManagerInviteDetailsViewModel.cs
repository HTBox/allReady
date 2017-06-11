using System;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.ManagerInvite
{
    public abstract class ManagerInviteDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Email")]
        public string InviteeEmailAddress { get; set; }

        [Display(Name = "Sent")]
        public DateTime SentDateTimeUtc { get; set; }

        [Display(Name = "Accepted")]
        public DateTime? AcceptedDateTimeUtc { get; set; }

        [Display(Name = "Rejected")]
        public DateTime? RejectedDateTimeUtc { get; set; }

        [Display(Name = "Revoked")]
        public DateTime? RevokedDateTimeUtc { get; set; }

        [Display(Name = "Message")]
        public string CustomMessage { get; set; }

        [Display(Name = "Sent by")]
        public string SenderUserEmail { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsPending { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int OrganizationId { get; set; }

        public string Status
        {
            get
            {
                if (IsAccepted) return "Accepted";
                else if (IsPending) return "Pending";
                else if (IsRejected) return "Rejected";
                else return "Revoked";
            }
        }
    }

    public class EventManagerInviteDetailsViewModel : ManagerInviteDetailsViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
    }

    public class CampaignManagerInviteDetailsViewModel : ManagerInviteDetailsViewModel
    {
    }
}