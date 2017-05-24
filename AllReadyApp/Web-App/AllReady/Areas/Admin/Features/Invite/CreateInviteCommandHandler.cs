using AllReady.Areas.Admin.ViewModels.Invite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Invite
{
    public class CreateInviteCommandHandler : AsyncRequestHandler<CreateInviteCommand>
    {
        private AllReadyContext _context;

        public CreateInviteCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        protected override async Task HandleCore(CreateInviteCommand message)
        {
            if (message?.Invite == null) throw new ArgumentException("InviteViewModel must be set");
            if (message?.UserId == null) throw new ArgumentException("User Id must be set");
            if (string.IsNullOrWhiteSpace(message?.Invite?.InviteeEmailAddress))
            {
                throw new ArgumentException("Invitee email address must be set");
            }

            switch (message.Invite.InviteType)
            {
                case ViewModels.Invite.InviteType.CampaignManagerInvite:
                    await CreateCampaignManagerInvite(message.Invite, message.UserId);
                    break;
                case ViewModels.Invite.InviteType.EventManagerInvite:
                    await CreateEventManagerInvite(message.Invite, message.UserId);
                    break;
                default:
                    break;
            }
        }

        private async Task CreateEventManagerInvite(InviteViewModel invite, string userId)
        {
            var @event = await GetEvent(invite.EventId);

            if (@event == null) throw new ArgumentException("EventId cannot be null for Event manager invite");

            var eventManagerInvite = new EventManagerInvite
            {
                InviteeEmailAddress = invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = invite.CustomMessage,
                SenderUserId = userId,
                Event = @event,
            };
            _context.EventManagerInvites.Add(eventManagerInvite);
            await _context.SaveChangesAsync();
        }

        private async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventId);
        }

        private async Task CreateCampaignManagerInvite(InviteViewModel invite, string userId)
        {
            var campaign = await GetCampaign(invite.CampaignId);
            if (campaign == null) throw new ArgumentException("CampaignId cannot be null for campaign manager invite");

            var campaignManagerInvite = new CampaignManagerInvite
            {
                InviteeEmailAddress = invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = invite.CustomMessage,
                SenderUserId = userId,
                CampaignId = invite.CampaignId,
            };
            _context.CampaignManagerInvites.Add(campaignManagerInvite);
            await _context.SaveChangesAsync();
        }

        private async Task<Campaign> GetCampaign(int campaignId)
        {
            return await _context.Campaigns.AsNoTracking().FirstOrDefaultAsync(c => c.Id == campaignId);
        }
    }
}
