using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class UserHasCampaignManagerInviteQueryHandler : IAsyncRequestHandler<UserHasCampaignManagerInviteQuery, bool>
    {
        private AllReadyContext _context;

        public UserHasCampaignManagerInviteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UserHasCampaignManagerInviteQuery message)
        {
            return await _context.CampaignManagerInvites.AsNoTracking()
                .AnyAsync(i => i.CampaignId == message.CampaignId &&
                               i.InviteeEmailAddress == message.InviteeEmail &&
                               i.IsPending);
        }
    }
}
