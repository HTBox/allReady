using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class EventManagerInviteDetailQueryHandler : IAsyncRequestHandler<EventManagerInviteDetailQuery, EventManagerInviteDetailsViewModel>
    {
        private AllReadyContext _context;

        public EventManagerInviteDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventManagerInviteDetailsViewModel> Handle(EventManagerInviteDetailQuery message)
        {
            var invite = await _context.EventManagerInvites.AsNoTracking()
                .Include(i => i.Event).ThenInclude(e => e.Campaign)
                .Include(i => i.SenderUser)
                .SingleOrDefaultAsync(i => i.Id == message.EventManagerInviteId);

            return new EventManagerInviteDetailsViewModel
            {
                Id = invite.Id,
                CampaignId = invite.Event.CampaignId,
                CampaignName = invite.Event.Campaign.Name,
                CustomMessage = invite.CustomMessage,
                EventId = invite.EventId,
                AcceptedDateTimeUtc = invite.AcceptedDateTimeUtc,
                EventName = invite.Event.Name,
                InviteeEmailAddress = invite.InviteeEmailAddress,
                IsAccepted = invite.IsAccepted,
                IsPending = invite.IsPending,
                IsRejected = invite.IsRejected,
                IsRevoked = invite.IsRevoked,
                RejectedDateTimeUtc = invite.RejectedDateTimeUtc,
                RevokedDateTimeUtc = invite.RevokedDateTimeUtc,
                SenderUserEmail = invite.SenderUser.Email,
                SentDateTimeUtc = invite.SentDateTimeUtc,
                OrganizationId = invite.Event.Campaign.ManagingOrganizationId,
            };
        }
    }
}
