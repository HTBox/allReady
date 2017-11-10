using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{

    public class AcceptDeclineEventManagerInviteDetailQueryHandler : IAsyncRequestHandler<AcceptDeclineEventManagerInviteDetailQuery, AcceptDeclineEventManagerInviteViewModel>
    {
        private readonly AllReadyContext _context;

        public AcceptDeclineEventManagerInviteDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<AcceptDeclineEventManagerInviteViewModel> Handle(AcceptDeclineEventManagerInviteDetailQuery message)
        {
            var invite = await _context.EventManagerInvites.AsNoTracking()
                .Include(i => i.Event)
                .ThenInclude(i => i.Campaign)
                .SingleOrDefaultAsync(i => i.Id == message.EventManagerInviteId);

            return new AcceptDeclineEventManagerInviteViewModel
            {
                CampaignName = invite.Event.Campaign.Name,
                EventName = invite.Event.Name,
                EventId = invite.Event.Id,
                InviteeEmailAddress = invite.InviteeEmailAddress,
                InviteId = invite.Id
            };
        }
    }
}
