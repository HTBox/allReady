namespace AllReady.Areas.Admin.ViewModels.ManagerInvite
{
    public abstract class AcceptDeclineManagerInviteViewModel
    {
        public int InviteId { get; set; }
        public string InviteeEmailAddress { get; set; }
        public string CampaignName { get; set; }
    }

    public class AcceptDeclineEventManagerInviteViewModel : AcceptDeclineManagerInviteViewModel
    {
        public string EventName { get; set; }
        public int EventId { get; set; }
    }

    public class AcceptDeclineCampaignManagerInviteViewModel : AcceptDeclineManagerInviteViewModel
    {
        public int CampaignId { get; set; }
    }

}
