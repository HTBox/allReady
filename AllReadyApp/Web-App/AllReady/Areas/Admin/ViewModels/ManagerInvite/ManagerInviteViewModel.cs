using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.ManagerInvite
{
    public abstract class ManagerInviteViewModel
    {
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string InviteeEmailAddress { get; set; }

        [Display(Name = "Custom Message")]
        public string CustomMessage { get; set; }

        public int CampaignId { get; set; }

        public string CampaignName { get; set; }

        public int OrganizationId { get; set; }
    }

    public class EventManagerInviteViewModel : ManagerInviteViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
    }

    public class CampaignManagerInviteViewModel : ManagerInviteViewModel
    {
    }
}
