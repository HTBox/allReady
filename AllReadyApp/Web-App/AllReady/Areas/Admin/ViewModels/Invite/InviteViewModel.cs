using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Invite
{
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

        public string FormAction { get; set; }

        public string Title { get; set; }
    }
}
