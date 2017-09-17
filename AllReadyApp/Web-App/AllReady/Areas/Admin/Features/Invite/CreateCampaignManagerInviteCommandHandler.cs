using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Invite
{
    public class CreateCampaignManagerInviteCommandHandler : AsyncRequestHandler<CreateCampaignManagerInviteCommand>
    {
        private AllReadyContext _context;

        public CreateCampaignManagerInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        protected override async Task HandleCore(CreateCampaignManagerInviteCommand message)
        {
            var campaign = await _context.Campaigns.AsNoTracking().SingleOrDefaultAsync(c => c.Id == message.Invite.CampaignId);
            if (campaign == null) throw new ArgumentException("CampaignId cannot be null for campaign manager invite");

            var campaignManagerInvite = new CampaignManagerInvite
            {
                InviteeEmailAddress = message.Invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = message.Invite.CustomMessage,
                SenderUserId = message.UserId,
                CampaignId = message.Invite.CampaignId,
            };
            _context.CampaignManagerInvites.Add(campaignManagerInvite);
            await _context.SaveChangesAsync();
        }
    }
}
