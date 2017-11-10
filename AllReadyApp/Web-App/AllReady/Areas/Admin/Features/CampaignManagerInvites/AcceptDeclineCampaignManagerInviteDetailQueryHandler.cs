using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptDeclineCampaignManagerInviteDetailQueryHandler : IAsyncRequestHandler<AcceptDeclineCampaignManagerInviteDetailQuery, AcceptDeclineCampaignManagerInviteViewModel>
    {
        private readonly AllReadyContext _context;

        public AcceptDeclineCampaignManagerInviteDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<AcceptDeclineCampaignManagerInviteViewModel> Handle(AcceptDeclineCampaignManagerInviteDetailQuery message)
        {
            var invite = await _context.CampaignManagerInvites.AsNoTracking()
                .Include(i => i.Campaign)
                .SingleOrDefaultAsync(i => i.Id == message.CampaignManagerInviteId);

            return new AcceptDeclineCampaignManagerInviteViewModel
            {
                CampaignName = invite.Campaign.Name,
                CampaignId = invite.CampaignId,
                InviteeEmailAddress = invite.InviteeEmailAddress,
                InviteId = invite.Id
            };
        }
    }
}
