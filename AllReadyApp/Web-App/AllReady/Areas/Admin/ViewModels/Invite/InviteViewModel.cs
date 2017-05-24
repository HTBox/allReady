using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels.Invite
{
    public enum InviteType
    {
        CampaignManagerInvite = 0,
        EventManagerInvite = 1,
    }

    public class InviteViewModel
    {
        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string InviteeEmailAddress { get; set; }

        [Display(Name = "Message")]
        public string CustomMessage { get; set; }

        public int EventId { get; set; }

        public int CampaignId { get; set; }

        public InviteType InviteType { get; set; }
    }
}
