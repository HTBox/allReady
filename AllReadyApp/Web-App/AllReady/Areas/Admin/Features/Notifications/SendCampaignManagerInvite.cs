using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class SendCampaignManagerInvite : IAsyncNotification
    {
        public string InviteeEmail { get; set; }
        public string Message { get; set; }
        public string CampaignName { get; set; }
        public string SenderName { get; set; }
        public string AcceptUrl { get; set; }
        public string DeclineUrl { get; set; }
        public string RegisterUrl { get; set; }
        public bool IsInviteeRegistered { get; set; }
    }
}
